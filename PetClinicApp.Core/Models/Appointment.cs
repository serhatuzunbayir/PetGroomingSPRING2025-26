namespace PetClinicApp.Core.Models;

// Randevu Türleri: Veteriner veya Bakım (Grooming) 
public enum AppointmentType { Veterinary, Grooming }

// Randevu Durumları [cite: 13]
public enum AppointmentStatus { Pending, Completed, Cancelled }

public class Appointment
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    
    public AppointmentType Type { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    // Hizmet bedeli ve ödeme durumu 
    public decimal ServiceFee { get; set; }
    public bool IsPaid { get; set; }

    // Hangi hayvana ait olduğu
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
}