namespace PetClinicApp.Core.Models;

/// <summary>
/// Randevu türünü belirler.
/// Veterinary: Veteriner muayenesi | Grooming: Tıraş ve bakım hizmeti
/// </summary>
public enum AppointmentType { Veterinary, Grooming }

/// <summary>
/// Randevunun mevcut durumunu belirler.
/// Pending: Bekliyor | Completed: Tamamlandı | Cancelled: İptal edildi
/// </summary>
public enum AppointmentStatus { Pending, Completed, Cancelled }

/// <summary>
/// Randevu varlık modeli.
/// Bir hayvana ait veteriner veya grooming randevusunu temsil eder.
/// ServiceFee ve IsPaid alanları ödeme takibi için kullanılır.
/// </summary>
public class Appointment
{
    /// <summary>Veritabanı birincil anahtarı (otomatik artar)</summary>
    public int Id { get; set; }

    /// <summary>Randevu tarihi ve saati</summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>Randevu türü: Veterinary veya Grooming</summary>
    public AppointmentType Type { get; set; }

    /// <summary>Randevu durumu — varsayılan olarak Pending (beklemede) başlar</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    /// <summary>Hizmet bedeli (TL/USD cinsinden)</summary>
    public decimal ServiceFee { get; set; }

    /// <summary>Ödeme durumu: true ise ödeme alınmış demektir</summary>
    public bool IsPaid { get; set; }

    /// <summary>Bu randevunun ait olduğu hayvanın Foreign Key'i</summary>
    public int PetId { get; set; }

    /// <summary>İlişkili hayvan nesnesi (Navigation Property — Eager Loading için)</summary>
    public Pet? Pet { get; set; }
}