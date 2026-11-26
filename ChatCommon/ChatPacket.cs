using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ChatCommon
{
    /// <summary>
    /// Ağ üzerinden gönderilen tek bir paket. Her paket tek satırlık metne çevrilir:
    /// <c>TÜR|alan1|alan2|...</c>. Alanlar URL kodlaması ile kaçışlanır; böylece
    /// mesaj içinde "|" veya satır sonu olsa bile protokol bozulmaz.
    /// </summary>
    public class ChatPacket
    {
        private const char Separator = '|';
        public const string TimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public PacketType Type { get; private set; }
        public IReadOnlyList<string> Fields { get; private set; }

        public ChatPacket(PacketType type, params string[] fields)
        {
            Type = type;
            Fields = (fields ?? new string[0]).Select(f => f ?? string.Empty).ToArray();
        }

        /// <summary>İstenen alanı döndürür; yoksa boş metin döner.</summary>
        public string Field(int index)
        {
            return index >= 0 && index < Fields.Count ? Fields[index] : string.Empty;
        }

        public string Serialize()
        {
            var parts = new List<string> { Type.ToString().ToUpperInvariant() };
            parts.AddRange(Fields.Select(Uri.EscapeDataString));
            return string.Join(Separator.ToString(), parts);
        }

        /// <summary>Gelen satırı pakete çevirir. Biçim bozuksa false döner.</summary>
        public static bool TryParse(string line, out ChatPacket packet)
        {
            packet = null;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] parts = line.Split(Separator);
            PacketType type;
            if (!Enum.TryParse(parts[0], true, out type) || !Enum.IsDefined(typeof(PacketType), type))
            {
                return false;
            }

            try
            {
                string[] fields = parts.Skip(1).Select(Uri.UnescapeDataString).ToArray();
                packet = new ChatPacket(type, fields);
                return true;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        // ---------- Sık kullanılan paketler için kısayollar ----------

        public static string Now()
        {
            return DateTime.Now.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime ParseTime(string value)
        {
            DateTime time;
            return DateTime.TryParseExact(value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time)
                ? time
                : DateTime.Now;
        }

        public static ChatPacket Hello(string userName)
        {
            return new ChatPacket(PacketType.Hello, userName);
        }

        public static ChatPacket Welcome(string userName)
        {
            return new ChatPacket(PacketType.Welcome, userName);
        }

        public static ChatPacket ErrorPacket(string text)
        {
            return new ChatPacket(PacketType.Error, text);
        }

        public static ChatPacket PublicMessage(string from, string text)
        {
            return new ChatPacket(PacketType.Message, from, text, Now());
        }

        public static ChatPacket PrivateMessage(string from, string to, string text)
        {
            return new ChatPacket(PacketType.Private, from, to, text, Now());
        }

        public static ChatPacket UserList(IEnumerable<string> users)
        {
            return new ChatPacket(PacketType.Users, users.ToArray());
        }

        public static ChatPacket SystemMessage(string text)
        {
            return new ChatPacket(PacketType.System, text, Now());
        }

        public static ChatPacket Bye()
        {
            return new ChatPacket(PacketType.Bye);
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}
