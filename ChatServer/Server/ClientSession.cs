using System;
using ChatCommon;

namespace ChatServer.Server
{
    /// <summary>Sunucuya bağlı tek bir istemcinin bilgileri.</summary>
    public class ClientSession
    {
        public ClientSession(ChatConnection connection)
        {
            Connection = connection;
            ConnectedAt = DateTime.Now;
            Address = connection.RemoteEndPoint != null ? connection.RemoteEndPoint.ToString() : "?";
        }

        public ChatConnection Connection { get; private set; }

        /// <summary>HELLO paketi gelene kadar null'dır.</summary>
        public string UserName { get; set; }

        public string Address { get; private set; }
        public DateTime ConnectedAt { get; private set; }
        public int MessageCount { get; set; }

        public bool IsRegistered
        {
            get { return !string.IsNullOrEmpty(UserName); }
        }
    }
}
