using System;

namespace ChatClient.Client
{
    /// <summary>Kullanıcıya doğrudan gösterilebilecek anlaşılır istemci hataları.</summary>
    public class ChatClientException : Exception
    {
        public ChatClientException(string message) : base(message)
        {
        }

        public ChatClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
