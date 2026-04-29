using Microsoft.EntityFrameworkCore;
using PetClinicApp.Core.Models;

namespace PetClinicApp.Core.Data;

public class AppDbContext : DbContext
{
    // Veritabanındaki tablolarımız
    public DbSet<Client> Clients { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public AppDbContext()
    {
        // ÖNEMLİ: Bu kod program ilk çalıştığında petclinic.db dosyası yoksa otomatik yaratır.
        
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // db'yi göreli yolla açınca Mac'te "unable to open database file" hatası alıyorduk,
        // yazılabilir bir klasöre yönlendirdim. Windows ve Web'de de sorunsuz çalışıyor.
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        Directory.CreateDirectory(folder);
        var dbPath = Path.Combine(folder, "petclinic.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}