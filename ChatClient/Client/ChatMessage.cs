using System;

namespace ChatClient.Client
{
    public enum ChatMessageKind
    {
        Public,
        Private,
        System,
        Local
    }

    /// <summary>Arayüzde gösterilen ve geçmişe kaydedilen tek bir sohbet satırı.</summary>
    public class ChatMessage
    {
        public ChatMessage(ChatMessageKind kind, string from, string to, string text, DateTime time)
        {
            Kind = kind;
            From = from;
            To = to;
            Text = text;
            Time = time;
        }

        public ChatMessageKind Kind { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public string Text { get; private set; }
        public DateTime Time { get; private set; }

        /// <summary>Mesajın başlık kısmı (ör. "ali → veli (özel)").</summary>
        public string Header
        {
            get
            {
                switch (Kind)
                {
                    case ChatMessageKind.Private:
                        return From + " → " + To + " (özel)";
                    case ChatMessageKind.System:
                    case ChatMessageKind.Local:
                        return "*";
                    default:
                        return From;
                }
            }
        }

        public string ToHistoryLine()
        {
            return "[" + Time.ToString("yyyy-MM-dd HH:mm:ss") + "] " + Header + ": " + Text;
        }
    }
}
