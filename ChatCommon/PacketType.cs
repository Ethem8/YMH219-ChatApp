namespace ChatCommon
{
    /// <summary>İstemci ile sunucu arasında gidip gelen paket türleri.</summary>
    public enum PacketType
    {
        /// <summary>İstemci → sunucu: kullanıcı adı ile katılma isteği. Alan: kullanıcı adı.</summary>
        Hello,

        /// <summary>Sunucu → istemci: katılma kabul edildi. Alan: kullanıcı adı.</summary>
        Welcome,

        /// <summary>Sunucu → istemci: hata (ör. kullanıcı adı kullanımda). Alan: açıklama.</summary>
        Error,

        /// <summary>Herkese açık mesaj. Alanlar: gönderen, metin, zaman.</summary>
        Message,

        /// <summary>Özel mesaj. Alanlar: gönderen, alıcı, metin, zaman.</summary>
        Private,

        /// <summary>Sunucu → istemci: çevrimiçi kullanıcı listesi. Alanlar: kullanıcı adları.</summary>
        Users,

        /// <summary>Sunucu → istemci: bilgi mesajı (katıldı/ayrıldı). Alanlar: metin, zaman.</summary>
        System,

        /// <summary>Bağlantıyı düzgün şekilde kapatma bildirimi.</summary>
        Bye
    }
}
