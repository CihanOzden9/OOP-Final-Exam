# Rent-A-Tech Kiralama Sistemi

Bu proje, C# ile geliştirilmiş konsol tabanlı bir teknoloji ürünü kiralama sistemidir. Kullanıcıların ürün ilanı vermesine, mevcut ürünleri kiralamasına ve yöneticilerin sistemi kontrol etmesine olanak tanır.

## Özellikler

### Kullanıcı Paneli
- **Giriş/Kayıt**: Kullanıcılar sisteme kayıt olabilir ve giriş yapabilir.
- **Ürün İlanı Verme**: Kullanıcılar kendi teknolojik ürünlerini kiralamak üzere sisteme ekleyebilir.
- **Ürün Kiralama**: Diğer kullanıcıların eklediği ürünleri belirli bir süre için kiralayabilir.
- **Geçmiş Görüntüleme**: Daha önce yapılan kiralamaların geçmişini ve toplam tutarlarını görebilir.

### Admin Paneli
- **Dashboard**: Sistemdeki toplam kullanıcı, ürün ve kiralama istatistiklerini görüntüler.
- **Kullanıcı Yönetimi**: Sistemdeki kullanıcıları listeler ve silebilir.
- **Ürün Yönetimi**: Uygunsuz veya eski ilanları sistemden kaldırabilir.

## Kurulum ve Çalıştırma

Bu projeyi çalıştırmak için bilgisayarınızda [.NET SDK](https://dotnet.microsoft.com/download) yüklü olmalıdır.

1. Proje klasörüne gidin:
   ```bash
   cd Odev
   ```

2. Projeyi çalıştırın:
   ```bash
   dotnet run
   ```

Veritabanı (`rental.db`) uygulama ilk kez çalıştırıldığında otomatik olarak oluşturulacaktır.

## Gereksinimler
- .NET 8.0 veya üzeri
- SQLite (System.Data.SQLite veya Microsoft.Data.Sqlite)

## Proje Yapısı
- **Program.cs**: Uygulamanın giriş noktası ve menü döngüsü.
- **Services/**: İş mantığını (Auth, Product, Rental, Admin) içeren sınıflar.
- **Models/**: Veritabanı tablolarını temsil eden sınıflar (User, Product, Rental).
- **Helpers/**: Veritabanı ve UI işlemlerini kolaylaştıran yardımcı sınıflar.
