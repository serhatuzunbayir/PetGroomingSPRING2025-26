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
        // Veritabanı olarak SQLite kullanacağımızı ve dosya adını belirtiyoruz
        optionsBuilder.UseSqlite("Data Source=petclinic.db");
    }
}