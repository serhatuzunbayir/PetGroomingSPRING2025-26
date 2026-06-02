namespace PetClinicApp.Core.Models;

/// <summary>
/// Determines the appointment type.
/// Veterinary: Vet checkup | Grooming: Shaving and care service
/// </summary>
public enum AppointmentType { Veterinary, Grooming }

/// <summary>
/// Determines the current status of the appointment.
/// Pending: Waiting | Completed: Done | Cancelled: Canceled
/// </summary>
public enum AppointmentStatus { Pending, Completed, Cancelled }

/// <summary>
/// Appointment entity model.
/// Represents a veterinary or grooming appointment for a pet.
/// ServiceFee and IsPaid fields are used for payment tracking.
/// </summary>
public class Appointment
{
    /// <summary>Database primary key (auto increment)</summary>
    public int Id { get; set; }

    /// <summary>Appointment date and time</summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>Appointment type: Veterinary or Grooming</summary>
    public AppointmentType Type { get; set; }

    /// <summary>Appointment status - starts as Pending by default</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    /// <summary>Service fee (in TL/USD)</summary>
    public decimal ServiceFee { get; set; }

    /// <summary>Payment status: true means payment received</summary>
    public bool IsPaid { get; set; }

    /// <summary>Foreign Key of the pet this appointment belongs to</summary>
    public int PetId { get; set; }

    /// <summary>Related pet object (Navigation Property for Eager Loading)</summary>
    public Pet? Pet { get; set; }
}