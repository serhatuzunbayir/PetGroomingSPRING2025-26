namespace PetClinicApp.Core.Models;

/// <summary>
/// Müşteri (hayvan sahibi) varlık modeli.
/// Bir müşterinin birden fazla evcil hayvanı olabilir (one-to-many ilişkisi).
/// </summary>
public class Client
{
    /// <summary>Veritabanı birincil anahtarı (otomatik artar)</summary>
    public int Id { get; set; }

    /// <summary>Müşterinin adı</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Müşterinin soyadı</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Müşterinin telefon numarası (iletişim için)</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Müşterinin e-posta adresi</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Müşteriye ait evcil hayvanlar koleksiyonu.
    /// One-to-Many ilişkisi: Bir müşterinin birden fazla hayvanı olabilir.
    /// </summary>
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}