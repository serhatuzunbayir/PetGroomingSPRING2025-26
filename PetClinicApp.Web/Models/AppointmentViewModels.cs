using PetClinicApp.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PetClinicApp.Web.Models;

/// <summary>
/// Randevu alma formu için ViewModel.
/// Müşteri ve hayvan seçimini, randevu detaylarını içerir.
/// </summary>
public class BookAppointmentViewModel
{
    // Formda seçim için mevcut listeler
    /// <summary>Dropdown'da gösterilecek tüm müşteriler</summary>
    public List<Client> Clients { get; set; } = new();

    /// <summary>Seçili müşteriye ait evcil hayvanlar</summary>
    public List<Pet> Pets { get; set; } = new();

    // Kullanıcının form üzerinde seçtiği değerler
    /// <summary>Randevu için seçilen evcil hayvanın ID'si</summary>
    [Required(ErrorMessage = "Please select a pet.")]
    public int SelectedPetId { get; set; }

    /// <summary>Randevu için seçilen müşterinin ID'si</summary>
    [Required(ErrorMessage = "Please select a client.")]
    public int SelectedClientId { get; set; }

    /// <summary>Randevu tarihi ve saati</summary>
    [Required(ErrorMessage = "Please select a date.")]
    public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

    /// <summary>Randevu türü: Veterinary veya Grooming</summary>
    [Required(ErrorMessage = "Please select an appointment type.")]
    public AppointmentType Type { get; set; }

    /// <summary>Hizmet bedeli (opsiyonel — 0 bırakılabilir)</summary>
    [Range(0, double.MaxValue, ErrorMessage = "Fee must be a positive number.")]
    public decimal ServiceFee { get; set; } = 0;

    /// <summary>Başarılı kayıt sonrası gösterilecek onay mesajı</summary>
    public string? ConfirmationMessage { get; set; }
}

/// <summary>
/// Müşterinin kendi randevularını görüntüleme sayfası için ViewModel.
/// </summary>
public class MyAppointmentsViewModel
{
    /// <summary>Sayfada görüntülenen müşteri</summary>
    public Client? SelectedClient { get; set; }

    /// <summary>Dropdown'da gösterilecek tüm müşteriler</summary>
    public List<Client> AllClients { get; set; } = new();

    /// <summary>Seçili müşterinin randevuları (LINQ ile filtrelenmiş)</summary>
    public List<Appointment> Appointments { get; set; } = new();
}
