using Microsoft.EntityFrameworkCore;
using PetClinicApp.Core.Models;

namespace PetClinicApp.Core.Data;

/// <summary>
/// Entity Framework Core database context class.
/// Configures the SQLite database and defines all tables.
/// Desktop and Web applications share the same DB file.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Client table</summary>
    public DbSet<Client> Clients { get; set; }

    /// <summary>Pet table</summary>
    public DbSet<Pet> Pets { get; set; }

    /// <summary>Appointment table</summary>
    public DbSet<Appointment> Appointments { get; set; }

    /// <summary>
    /// Default constructor: Auto-creates database if it doesn't exist.
    /// </summary>
    public AppDbContext()
    {
        // Auto-create petclinic.db if it doesn't exist when app runs.
        Database.EnsureCreated();
    }

    /// <summary>
    /// Configures the database connection.
    /// Uses a relative path to "Documents/PetClinicApp" folder
    /// so it works on any computer.
    /// Desktop and Web apps share this common location.
    /// </summary>
    /// <param name="optionsBuilder">EF Core configuration object</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use My Documents/PetClinicApp/ folder - works consistently everywhere.
        // This allows Desktop and Web apps to share the same DB file.
        var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var appFolder = Path.Combine(documentsFolder, "PetClinicApp");

        // Create folder if it doesn't exist
        Directory.CreateDirectory(appFolder);

        // Relative logic: Documents/PetClinicApp/petclinic.db
        var dbPath = Path.Combine(appFolder, "petclinic.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}