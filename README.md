# YMH219 - İstemci-Sunucu Chat Uygulaması

`TcpListener` / `TcpClient` ile çalışan, Windows Forms arayüzlü bir sohbet sistemi. Bir sunucu uygulaması ve aynı bilgisayardan ya da yerel ağdan bağlanabilen istemci uygulamalarından oluşur. Birden fazla istemci aynı odada mesajlaşabilir ve birbirine özel mesaj gönderebilir.

> YMH 219 Nesne Tabanlı Programlama dersi, Ek Uygulama Ödevi 3

## Kullanılan teknolojiler

- C# / .NET 8, Windows Forms
- `System.Net.Sockets`: `TcpListener`, `TcpClient`, `NetworkStream`
- `StreamReader` / `StreamWriter` ile satır tabanlı protokol
- `async` / `await`, `Task`, `SemaphoreSlim`
- Harici NuGet paketi yok

## Proje yapısı

```
YMH219-ChatApp.sln
├── ChatCommon/   Ortak sınıf kütüphanesi (protokol + bağlantı)
├── ChatServer/   Sunucu (Windows Forms)
└── ChatClient/   İstemci (Windows Forms)
```

| Sınıf | Görevi |
|---|---|
| `ChatCommon/PacketType` | Paket türleri: `Hello`, `Welcome`, `Error`, `Message`, `Private`, `Users`, `System`, `Bye` |
| `ChatCommon/ChatPacket` | Paketi `TÜR\|alan1\|alan2` biçiminde tek satıra çevirir, satırı geri ayrıştırır. Alanlar URL kodlamasıyla kaçışlanır, bu yüzden mesajdaki `\|` veya satır sonu protokolü bozmaz. |
| `ChatCommon/ChatConnection` | `TcpClient` üzerinde asenkron paket okuma/yazma. Yazmalar `SemaphoreSlim` ile sıraya alınır. |
| `ChatServer/Server/ChatServerHost` | Bağlantıları kabul eder. Her istemciyi ayrı bir görevde dinler. Kullanıcı adı doğrular, mesajları yayınlar, özel mesajları iletir. |
| `ChatServer/Server/ClientSession` | Bağlı istemcinin kullanıcı adı, adresi, bağlanma zamanı ve mesaj sayısı |
| `ChatClient/Client/ChatClientService` | Bağlanma (zaman aşımlı), giriş, mesaj gönderme ve gelen paketleri olaylara çevirme |
| `ChatClient/Client/ChatMessage` | Ekranda gösterilen ve geçmişe yazılan mesaj modeli |

## Kurulum ve çalıştırma

1. `git clone https://github.com/Ethem8/YMH219-ChatApp.git`
2. `YMH219-ChatApp.sln` dosyasını Visual Studio 2022 ile açın.
3. Solution'a sağ tıklayın, **Configure Startup Projects…** → **Multiple startup projects** seçin. `ChatServer` ve `ChatClient` için *Start* seçip **F5**'e basın.
4. Sunucu penceresinde **Sunucuyu Başlat**'a basın (varsayılan port 5000).
5. İstemcide sunucu adresi (aynı bilgisayar için `127.0.0.1`) ve kullanıcı adı girip **Bağlan**'a basın.
6. İkinci bir istemci açmak için `ChatClient\bin\Debug\net8.0-windows\ChatClient.exe` dosyasını bir kez daha çalıştırın.

Komut satırından:

```
dotnet run --project ChatServer
dotnet run --project ChatClient
```

> Farklı bilgisayarlardan bağlanmak için sunucu penceresinin üstünde yazan yerel IP adresini kullanın. Windows Güvenlik Duvarı izin isterse **Özel ağlar** için izin verin.

## Özellikler

**İstemci**
- Kullanıcı adı ile bağlanma. Aynı ad kullanımdaysa veya ad geçersizse sunucu reddeder ve sebebi gösterilir.
- **Bağlan**, **Gönder** (Enter tuşu da çalışır) ve **Bağlantıyı Kes** işlevleri
- Gönderilen ve alınan mesajlar zaman bilgisiyle, renkli olarak gösterilir. Kendi mesajlarınız yeşil, başkalarınınki mavi, özel mesajlar mor, sistem mesajları gri.
- Bağlantı durumu göstergesi (● Çevrimiçi / ● Çevrimdışı)
- Bağlantı hataları (sunucu kapalı, zaman aşımı, bağlantının kopması) uygulamayı kapatmaz, kullanıcıya mesajla bildirilir.
- *(İsteğe bağlı)* Çevrimiçi kullanıcı listesi. Birden fazla istemci aynı odaya katılabilir.
- *(İsteğe bağlı)* **Özel mesaj:** "Alıcı" listesinden kullanıcı seçin ya da listede kullanıcıya çift tıklayın.
- *(İsteğe bağlı)* **Geçmişi Kaydet:** Mesaj geçmişi `.txt` dosyasına yazılır.

**Sunucu**
- Port seçimi, Başlat/Durdur
- Bağlı istemcilerin kullanıcı adı, IP:port, bağlanma zamanı ve mesaj sayısı listelenir.
- Bağlantı, katılma, ayrılma ve mesaj olaylarını gösteren sunucu günlüğü
- Port kullanımdaysa uygulama çökmez, hata mesajı gösterir.
- Gönderen adı istemcinin gönderdiği pakete göre değil, sunucudaki oturuma göre belirlenir. Bu sayede kimse başkası adına mesaj atamaz.

## Protokol örneği

```
HELLO|ali                         istemci → sunucu
WELCOME|ali                       sunucu → istemci
USERS|ali|veli                    sunucu → herkes
MESSAGE|ali|merhaba|2026-11-26T17:46:46
PRIVATE|ali|veli|gizli%20mesaj|2026-11-26T17:46:47
SYSTEM|veli%20sohbete%20kat%C4%B1ld%C4%B1.|2026-11-26T17:46:46
BYE
```

## Ekran görüntüleri

| Sunucu | İki istemci mesajlaşırken |
|---|---|
| ![Sunucu](screenshots/sunucu.png) | ![İstemciler](screenshots/istemciler.png) |

## Bilinen sınırlamalar / geliştirme notları

- Trafik şifrelenmez (TLS yok), bu yüzden yalnızca yerel ağ/ders amaçlı kullanım içindir.
- Tek bir ortak oda vardır. Birden fazla oda desteği eklenebilir.
- Mesaj geçmişi sunucuda tutulmaz. Her istemci kendi geçmişini kaydeder.

## Öğrenci

**İbrahim Ethem Erkan** · 240541010
