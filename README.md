🎵 Sıyırdı - Müzik Karar Verici
Sıyırdı, müzik dinlemek istediğinizde "Ne dinlesem?" derdine son veren, favori şarkılarınızı ve linklerini saklayıp sizin yerinize rastgele seçim yapan bir masaüstü uygulamasıdır.

🚀 Özellikler
Rastgele Şarkı Seçimi: Kaydettiğiniz şarkılar arasından sizin için rastgele birini seçer ve tarayıcıda otomatik açar.

İki Farklı Liste: Şarkıları "Genel Liste" (Tüm Şarkılar) veya "Favorilerim" (MySarkiInfo) olarak iki ayrı kategoride saklayabilirsiniz.

Kolay Ekleme/Silme: Şarkı ismi ve URL girerek veritabanına hızlıca kayıt ekleyebilir veya silebilirsiniz.

URL Doğrulama: Girilen linkin geçerli bir web adresi olup olmadığını (http/https) kontrol eder.

Platform Desteği: YouTube, Spotify, SoundCloud ve Apple Music linklerini otomatik tanır.

🛠️ Teknolojiler
Bu proje aşağıdaki teknolojiler kullanılarak geliştirilmiştir:

Dil: C# (.NET Framework 4.7.2).

Arayüz: Windows Forms (WinForms).

Veritabanı: SQLite & Entity Framework 6.

Kütüphaneler: System.Data.SQLite.

⚙️ Kurulum ve Çalıştırma
Projeyi bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyin:

Projeyi Klonlayın:

Bash

git clone https://github.com/kullaniciadi/siyirdi.git
Visual Studio ile Açın: Sıyırdı.sln veya Sıyırdı.csproj dosyasını Visual Studio 2019/2022 ile açın.

Paketleri Yükleyin: NuGet Package Manager üzerinden gerekli SQLite paketlerini geri yükleyin (Restore).

Veritabanı Ayarı (Önemli!): Uygulama varsayılan olarak Sıyırdı.db dosyasını Masaüstü (Desktop) klasöründe arar. Eğer veritabanı dosyası yoksa uygulama hata verebilir.

Projeyi derlediğinizde veritabanı dosyasının masaüstünde olduğundan emin olun.

Başlatın: Projeyi F5 tuşu ile veya "Start" butonuna basarak çalıştırın.

💻 Kullanım
Şarkı Ekleme:

Üst kısımdaki kutucuğa Şarkı İsmi yazın.

Alt kısımdaki kutucuğa YouTube/Spotify Linkini yapıştırın.

Soldaki "Ekle" butonuna basarak genel listeye, sağdaki "Listeme Ekle" butonuna basarak favorilere ekleyin.

Şarkı Çalma:

"Rastgele Şarkı Aç" butonuna bastığınızda uygulama veritabanından rastgele bir satır çeker ve varsayılan tarayıcınızda o linki açar.

📸 Ekran Görüntüleri

![Uygulama Ana Ekran Görüntüsü](images/Ana-Ekran.png)

*Uygulamanın ana arayüzü.*

![Tüm Şarkıları Görme Ekranı](images/Liste.png)

*Tüm Şarkılar
