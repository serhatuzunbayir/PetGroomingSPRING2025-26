using PetClinicApp.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PetClinicApp.Web.Models;

/// <summary>
/// ViewModel for the appointment booking form.
/// Contains client and pet selection, and appointment details.
/// </summary>
public class BookAppointmentViewModel
{
    // Lists available for selection in the form
    /// <summary>All clients to be shown in the dropdown</summary>
    public List<Client> Clients { get; set; } = new();

    /// <summary>Pets belonging to the selected client</summary>
    public List<Pet> Pets { get; set; } = new();

    // Values selected by the user on the form
    /// <summary>ID of the pet selected for the appointment</summary>
    [Required(ErrorMessage = "Please select a pet.")]
    public int SelectedPetId { get; set; }

    /// <summary>ID of the client selected for the appointment</summary>
    [Required(ErrorMessage = "Please select a client.")]
    public int SelectedClientId { get; set; }

    /// <summary>Appointment date and time</summary>
    [Required(ErrorMessage = "Please select a date.")]
    public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

    /// <summary>Appointment type: Veterinary or Grooming</summary>
    [Required(ErrorMessage = "Please select an appointment type.")]
    public AppointmentType Type { get; set; }

    /// <summary>Service fee (optional - can be left as 0)</summary>
    [Range(0, double.MaxValue, ErrorMessage = "Fee must be a positive number.")]
    public decimal ServiceFee { get; set; } = 0;

    /// <summary>Confirmation message to show after successful booking</summary>
    public string? ConfirmationMessage { get; set; }
}

/// <summary>
/// ViewModel for the page where a client views their own appointments.
/// </summary>
public class MyAppointmentsViewModel
{
    /// <summary>The client displayed on the page</summary>
    public Client? SelectedClient { get; set; }

    /// <summary>All clients to be shown in the dropdown</summary>
    public List<Client> AllClients { get; set; } = new();

    /// <summary>Appointments of the selected client (filtered via LINQ)</summary>
    public List<Appointment> Appointments { get; set; } = new();
}
