using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatCommon;

namespace ChatClient.Client
{
    /// <summary>
    /// Sunucuya bağlanan, mesaj gönderen ve gelen paketleri olaylara çeviren istemci sınıfı.
    /// ConnectAsync UI iş parçacığından çağrıldığında olaylar da UI iş parçacığında tetiklenir
    /// (await, SynchronizationContext'e geri döner).
    /// </summary>
    public class ChatClientService
    {
        private const int ConnectTimeoutMs = 5000;

        private ChatConnection _connection;
        private bool _closingByUser;

        public string UserName { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }

        public bool IsConnected
        {
            get { return _connection != null && _connection.IsConnected; }
        }

        public event EventHandler<ChatMessage> MessageReceived;
        public event EventHandler<IReadOnlyList<string>> UsersChanged;
        public event EventHandler<string> ErrorReceived;

        /// <summary>Bağlantı koptuğunda tetiklenir. Argüman: sebep.</summary>
        public event EventHandler<string> Disconnected;

        /// <summary>
        /// Sunucuya bağlanır ve kullanıcı adıyla giriş yapar. Başarısız olursa
        /// anlaşılır bir mesajla <see cref="ChatClientException"/> fırlatır.
        /// </summary>
        public async Task ConnectAsync(string host, int port, string userName)
        {
            if (IsConnected)
            {
                throw new ChatClientException("Zaten bağlısınız.");
            }

            var tcp = new TcpClient();
            try
            {
                Task connectTask = tcp.ConnectAsync(host, port);
                if (await Task.WhenAny(connectTask, Task.Delay(ConnectTimeoutMs)) != connectTask)
                {
                    throw new ChatClientException("Sunucuya bağlanırken zaman aşımı oluştu (" + host + ":" + port + ").");
                }
                await connectTask; // Bağlantı hatası varsa burada fırlatılır.
            }
            catch (SocketException ex)
            {
                tcp.Close();
                throw new ChatClientException("Sunucuya bağlanılamadı: " + ex.Message, ex);
            }
            catch (ChatClientException)
            {
                tcp.Close();
                throw;
            }

            var connection = new ChatConnection(tcp);
            try
            {
                await connection.SendAsync(ChatPacket.Hello(userName));
                ChatPacket reply = await connection.ReadPacketAsync();

                if (reply == null)
                {
                    throw new ChatClientException("Sunucu bağlantıyı kapattı.");
                }
                if (reply.Type == PacketType.Error)
                {
                    throw new ChatClientException(reply.Field(0));
                }
                if (reply.Type != PacketType.Welcome)
                {
                    throw new ChatClientException("Sunucudan beklenmeyen yanıt geldi.");
                }

                UserName = reply.Field(0);
            }
            catch (IOException ex)
            {
                connection.Dispose();
                throw new ChatClientException("Giriş sırasında bağlantı koptu: " + ex.Message, ex);
            }
            catch (ChatClientException)
            {
                connection.Dispose();
                throw;
            }

            Host = host;
            Port = port;
            _connection = connection;
            _closingByUser = false;

            // Dinleme döngüsü beklenmeden başlatılır.
            _ = ReceiveLoopAsync(connection);
        }

        public Task SendPublicAsync(string text)
        {
            return SendAsync(ChatPacket.PublicMessage(UserName, text));
        }

        public Task SendPrivateAsync(string toUser, string text)
        {
            return SendAsync(ChatPacket.PrivateMessage(UserName, toUser, text));
        }

        private async Task SendAsync(ChatPacket packet)
        {
            if (!IsConnected)
            {
                throw new ChatClientException("Sunucuya bağlı değilsiniz.");
            }
            try
            {
                await _connection.SendAsync(packet);
            }
            catch (IOException ex)
            {
                HandleDisconnect("Mesaj gönderilemedi, bağlantı koptu.");
                throw new ChatClientException("Mesaj gönderilemedi: " + ex.Message, ex);
            }
            catch (ObjectDisposedException)
            {
                throw new ChatClientException("Bağlantı kapalı.");
            }
        }

        public async Task DisconnectAsync()
        {
            if (_connection == null) return;
            _closingByUser = true;
            await _connection.TrySendAsync(ChatPacket.Bye());
            HandleDisconnect("Bağlantı kesildi.");
        }

        private async Task ReceiveLoopAsync(ChatConnection connection)
        {
            string reason = "Sunucu bağlantıyı kapattı.";
            try
            {
                while (true)
                {
                    ChatPacket packet = await connection.ReadPacketAsync();
                    if (packet == null)
                    {
                        break;
                    }
                    Dispatch(packet);
                }
            }
            catch (IOException)
            {
                reason = "Sunucu ile bağlantı beklenmedik şekilde koptu.";
            }
            catch (ObjectDisposedException)
            {
                // Kullanıcı bağlantıyı kendisi kesti.
            }

            if (connection == _connection && !_closingByUser)
            {
                HandleDisconnect(reason);
            }
        }

        private void Dispatch(ChatPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.Message:
                    Raise(MessageReceived, new ChatMessage(ChatMessageKind.Public,
                        packet.Field(0), null, packet.Field(1), ChatPacket.ParseTime(packet.Field(2))));
                    break;

                case PacketType.Private:
                    Raise(MessageReceived, new ChatMessage(ChatMessageKind.Private,
                        packet.Field(0), packet.Field(1), packet.Field(2), ChatPacket.ParseTime(packet.Field(3))));
                    break;

                case PacketType.System:
                    Raise(MessageReceived, new ChatMessage(ChatMessageKind.System,
                        null, null, packet.Field(0), ChatPacket.ParseTime(packet.Field(1))));
                    break;

                case PacketType.Users:
                    var handler = UsersChanged;
                    if (handler != null) handler(this, packet.Fields.Where(f => f.Length > 0).ToList());
                    break;

                case PacketType.Error:
                    Raise(ErrorReceived, packet.Field(0));
                    break;
            }
        }

        private void HandleDisconnect(string reason)
        {
            var connection = _connection;
            if (connection == null) return;
            _connection = null;
            connection.Dispose();
            Raise(Disconnected, reason);
        }

        private void Raise<T>(EventHandler<T> handler, T args)
        {
            if (handler != null) handler(this, args);
        }
    }
}
