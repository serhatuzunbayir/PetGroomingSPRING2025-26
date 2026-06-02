using Microsoft.EntityFrameworkCore;
using PetClinicApp.Core.Models;

namespace PetClinicApp.Core.Data;

/// <summary>
/// Entity Framework Core veritabanı bağlam sınıfı.
/// SQLite veritabanını yapılandırır ve tüm tabloları tanımlar.
/// Desktop ve Web uygulamaları aynı DB dosyasını paylaşır.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Müşteri (Client) tablosu</summary>
    public DbSet<Client> Clients { get; set; }

    /// <summary>Evcil hayvan (Pet) tablosu</summary>
    public DbSet<Pet> Pets { get; set; }

    /// <summary>Randevu (Appointment) tablosu</summary>
    public DbSet<Appointment> Appointments { get; set; }

    /// <summary>
    /// Varsayılan constructor: Veritabanı yoksa otomatik olarak oluşturur.
    /// </summary>
    public AppDbContext()
    {
        // Uygulama ilk çalıştığında petclinic.db dosyası yoksa otomatik yaratılır.
        Database.EnsureCreated();
    }

    /// <summary>
    /// Veritabanı bağlantısını yapılandırır.
    /// Herhangi bir bilgisayarda çalışabilmesi için kullanıcının
    /// "Documents/PetClinicApp" klasörüne göre relative path kullanır.
    /// Desktop ve Web uygulamaları bu ortak konumu paylaşır.
    /// </summary>
    /// <param name="optionsBuilder">EF Core yapılandırma nesnesi</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // My Documents/PetClinicApp/ klasörünü kullan — her bilgisayarda tutarlı çalışır.
        // Bu sayede Desktop ve Web uygulamaları aynı veritabanı dosyasını paylaşır.
        var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var appFolder = Path.Combine(documentsFolder, "PetClinicApp");

        // Klasör yoksa oluştur
        Directory.CreateDirectory(appFolder);

        // Relative mantığıyla: Documents/PetClinicApp/petclinic.db
        var dbPath = Path.Combine(appFolder, "petclinic.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}