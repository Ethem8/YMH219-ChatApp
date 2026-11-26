using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatCommon;

namespace ChatServer.Server
{
    /// <summary>
    /// TcpListener ile istemcileri kabul eden, mesajları ilgili kişilere ileten sohbet sunucusu.
    /// Arayüzden bağımsızdır; olaylar arka plan iş parçacığından tetiklenebilir.
    /// </summary>
    public class ChatServerHost
    {
        public const int MaxUserNameLength = 20;

        private readonly object _sync = new object();
        private readonly List<ClientSession> _sessions = new List<ClientSession>();
        private TcpListener _listener;

        public bool IsRunning { get; private set; }
        public int Port { get; private set; }

        /// <summary>Sunucu günlüğüne yazılacak bir satır üretildiğinde tetiklenir.</summary>
        public event EventHandler<string> Log;

        /// <summary>Bağlı istemci listesi değiştiğinde tetiklenir.</summary>
        public event EventHandler ClientsChanged;

        /// <summary>Kayıtlı (kullanıcı adı almış) istemcilerin anlık kopyası.</summary>
        public List<ClientSession> GetClients()
        {
            lock (_sync)
            {
                return _sessions.Where(s => s.IsRegistered).ToList();
            }
        }

        public void Start(int port)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Sunucu zaten çalışıyor.");
            }

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start(); // Port kullanımdaysa SocketException fırlatır; form yakalar.
            Port = port;
            IsRunning = true;
            WriteLog("Sunucu " + port + " portunda dinlemeye başladı.");

            // Bağlantı kabul döngüsü arka planda çalışır.
            Task.Run(() => AcceptLoopAsync());
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;

            try { _listener.Stop(); } catch (SocketException) { }

            List<ClientSession> sessions;
            lock (_sync)
            {
                sessions = _sessions.ToList();
                _sessions.Clear();
            }
            foreach (var session in sessions)
            {
                session.Connection.TrySendAsync(ChatPacket.SystemMessage("Sunucu kapatıldı.")).Wait(500);
                session.Connection.Dispose();
            }

            WriteLog("Sunucu durduruldu.");
            OnClientsChanged();
        }

        private async Task AcceptLoopAsync()
        {
            while (IsRunning)
            {
                TcpClient tcp;
                try
                {
                    tcp = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                    break; // Stop() çağrıldı.
                }
                catch (SocketException)
                {
                    if (!IsRunning) break;
                    continue;
                }

                var session = new ClientSession(new ChatConnection(tcp));
                lock (_sync)
                {
                    _sessions.Add(session);
                }
                WriteLog("Yeni bağlantı: " + session.Address);

                // Her istemci kendi görevinde dinlenir.
                _ = Task.Run(() => HandleClientAsync(session));
            }
        }

        private async Task HandleClientAsync(ClientSession session)
        {
            try
            {
                while (IsRunning)
                {
                    ChatPacket packet = await session.Connection.ReadPacketAsync().ConfigureAwait(false);
                    if (packet == null || packet.Type == PacketType.Bye)
                    {
                        break;
                    }
                    await ProcessPacketAsync(session, packet).ConfigureAwait(false);
                }
            }
            catch (IOException)
            {
                // İstemci beklenmedik şekilde koptu.
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                await RemoveSessionAsync(session).ConfigureAwait(false);
            }
        }

        private async Task ProcessPacketAsync(ClientSession session, ChatPacket packet)
        {
            if (!session.IsRegistered && packet.Type != PacketType.Hello)
            {
                await session.Connection.TrySendAsync(ChatPacket.ErrorPacket("Önce kullanıcı adı ile giriş yapın.")).ConfigureAwait(false);
                return;
            }

            switch (packet.Type)
            {
                case PacketType.Hello:
                    await RegisterAsync(session, packet.Field(0).Trim()).ConfigureAwait(false);
                    break;

                case PacketType.Message:
                    string text = packet.Field(1);
                    if (string.IsNullOrWhiteSpace(text)) return;
                    session.MessageCount++;
                    // Gönderen adı istemciden değil oturumdan alınır; başkası adına mesaj atılamaz.
                    await BroadcastAsync(ChatPacket.PublicMessage(session.UserName, text)).ConfigureAwait(false);
                    WriteLog(session.UserName + ": " + text);
                    OnClientsChanged();
                    break;

                case PacketType.Private:
                    await SendPrivateAsync(session, packet.Field(1), packet.Field(2)).ConfigureAwait(false);
                    break;
            }
        }

        private async Task RegisterAsync(ClientSession session, string userName)
        {
            string error = ValidateUserName(userName);
            if (error != null)
            {
                await session.Connection.TrySendAsync(ChatPacket.ErrorPacket(error)).ConfigureAwait(false);
                return;
            }

            lock (_sync)
            {
                bool taken = _sessions.Any(s => s != session &&
                    string.Equals(s.UserName, userName, StringComparison.OrdinalIgnoreCase));
                if (!taken)
                {
                    session.UserName = userName;
                }
            }

            if (!session.IsRegistered)
            {
                await session.Connection.TrySendAsync(ChatPacket.ErrorPacket("\"" + userName + "\" kullanıcı adı kullanımda.")).ConfigureAwait(false);
                return;
            }

            await session.Connection.TrySendAsync(ChatPacket.Welcome(userName)).ConfigureAwait(false);
            WriteLog(userName + " (" + session.Address + ") sohbete katıldı.");
            await BroadcastAsync(ChatPacket.SystemMessage(userName + " sohbete katıldı.")).ConfigureAwait(false);
            await BroadcastUserListAsync().ConfigureAwait(false);
            OnClientsChanged();
        }

        private static string ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return "Kullanıcı adı boş olamaz.";
            if (userName.Length > MaxUserNameLength)
                return "Kullanıcı adı en fazla " + MaxUserNameLength + " karakter olabilir.";
            if (userName.Contains(",") || userName.Contains(" "))
                return "Kullanıcı adında boşluk veya virgül olamaz.";
            return null;
        }

        private async Task SendPrivateAsync(ClientSession from, string toUser, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            ClientSession target;
            lock (_sync)
            {
                target = _sessions.FirstOrDefault(s => s.IsRegistered &&
                    string.Equals(s.UserName, toUser, StringComparison.OrdinalIgnoreCase));
            }

            if (target == null)
            {
                await from.Connection.TrySendAsync(ChatPacket.ErrorPacket("\"" + toUser + "\" adlı kullanıcı çevrimiçi değil.")).ConfigureAwait(false);
                return;
            }

            from.MessageCount++;
            var packet = ChatPacket.PrivateMessage(from.UserName, target.UserName, text);
            await target.Connection.TrySendAsync(packet).ConfigureAwait(false);
            if (target != from)
            {
                // Gönderen de kendi özel mesajını görsün.
                await from.Connection.TrySendAsync(packet).ConfigureAwait(false);
            }
            WriteLog("[Özel] " + from.UserName + " → " + target.UserName);
            OnClientsChanged();
        }

        private async Task RemoveSessionAsync(ClientSession session)
        {
            bool removed;
            lock (_sync)
            {
                removed = _sessions.Remove(session);
            }
            session.Connection.Dispose();

            if (!removed) return;

            if (session.IsRegistered)
            {
                WriteLog(session.UserName + " ayrıldı.");
                await BroadcastAsync(ChatPacket.SystemMessage(session.UserName + " sohbetten ayrıldı.")).ConfigureAwait(false);
                await BroadcastUserListAsync().ConfigureAwait(false);
            }
            else
            {
                WriteLog("Bağlantı kapandı: " + session.Address);
            }
            OnClientsChanged();
        }

        private async Task BroadcastAsync(ChatPacket packet)
        {
            List<ClientSession> targets;
            lock (_sync)
            {
                targets = _sessions.Where(s => s.IsRegistered).ToList();
            }
            foreach (var target in targets)
            {
                await target.Connection.TrySendAsync(packet).ConfigureAwait(false);
            }
        }

        private Task BroadcastUserListAsync()
        {
            List<string> names;
            lock (_sync)
            {
                names = _sessions.Where(s => s.IsRegistered).Select(s => s.UserName).OrderBy(n => n).ToList();
            }
            return BroadcastAsync(ChatPacket.UserList(names));
        }

        private void WriteLog(string text)
        {
            var handler = Log;
            if (handler != null) handler(this, "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
        }

        private void OnClientsChanged()
        {
            var handler = ClientsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
