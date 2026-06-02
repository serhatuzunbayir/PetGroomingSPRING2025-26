namespace PetClinicApp.Core.Models;

/// <summary>
/// Evcil hayvan varlık modeli.
/// Bir müşteriye ait (Foreign Key: ClientId) hayvanı temsil eder.
/// Bir hayvanın birden fazla randevusu olabilir (one-to-many ilişkisi).
/// </summary>
public class Pet
{
    /// <summary>Veritabanı birincil anahtarı (otomatik artar)</summary>
    public int Id { get; set; }

    /// <summary>Hayvanın adı (örn: Karabaş, Pamuk)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Hayvanın türü (örn: Dog, Cat, Bird)</summary>
    public string Species { get; set; } = string.Empty;

    /// <summary>Hayvanın yaşı (yıl cinsinden)</summary>
    public int Age { get; set; }

    /// <summary>Hayvanın cinsiyeti (Male/Female)</summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Hayvanın klinik/bakım notları.
    /// Veteriner muayene sonuçları veya grooming notları buraya girilir.
    /// Örn: "Kuduz aşısı yapıldı", "Tırnak kesimi yapıldı"
    /// </summary>
    public string ClinicalNotes { get; set; } = string.Empty;

    /// <summary>Bu hayvanın sahibini belirten Foreign Key (ClientId)</summary>
    public int ClientId { get; set; }

    /// <summary>İlişkili müşteri nesnesi (Navigation Property — Eager Loading için)</summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Bu hayvana ait randevular koleksiyonu.
    /// One-to-Many ilişkisi: Bir hayvanın birden fazla randevusu olabilir.
    /// </summary>
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}