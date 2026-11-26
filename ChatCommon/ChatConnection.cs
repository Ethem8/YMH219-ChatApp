using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatCommon
{
    /// <summary>
    /// TcpClient üzerine kurulu, satır tabanlı paket okuma/yazma sınıfı.
    /// Hem sunucu hem istemci tarafında kullanılır.
    /// </summary>
    public class ChatConnection : IDisposable
    {
        private readonly TcpClient _client;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        // Aynı anda iki farklı yerden yazılırsa satırlar karışmasın diye yazmalar sıraya alınır.
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public ChatConnection(TcpClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;
            NetworkStream stream = client.GetStream();
            var encoding = new UTF8Encoding(false);
            _reader = new StreamReader(stream, encoding);
            _writer = new StreamWriter(stream, encoding) { AutoFlush = true, NewLine = "\n" };
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        }

        public IPEndPoint RemoteEndPoint { get; private set; }

        public bool IsConnected
        {
            get { return !_disposed && _client.Connected; }
        }

        /// <summary>
        /// Bir sonraki geçerli paketi okur. Bağlantı karşı taraftan kapatıldıysa null döner.
        /// Bozuk satırlar sessizce atlanır.
        /// </summary>
        public async Task<ChatPacket> ReadPacketAsync()
        {
            while (true)
            {
                string line = await _reader.ReadLineAsync().ConfigureAwait(false);
                if (line == null)
                {
                    return null;
                }

                ChatPacket packet;
                if (ChatPacket.TryParse(line, out packet))
                {
                    return packet;
                }
            }
        }

        public async Task SendAsync(ChatPacket packet)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("ChatConnection");
            }

            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await _writer.WriteLineAsync(packet.Serialize()).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>Gönderim hatasını yutar; kapanmakta olan bağlantılar için kullanılır.</summary>
        public async Task<bool> TrySendAsync(ChatPacket packet)
        {
            try
            {
                await SendAsync(packet).ConfigureAwait(false);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _client.Close(); } catch (SocketException) { }
            _writeLock.Dispose();
        }
    }
}
