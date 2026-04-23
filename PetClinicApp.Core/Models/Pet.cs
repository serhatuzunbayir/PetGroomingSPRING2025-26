namespace PetClinicApp.Core.Models;

public class Pet
{
    public int Id { get; set; }
    // İsterlerdeki özellikler: İsim, Tür, Yaş, Cinsiyet [cite: 11]
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;

    // Klinik ve bakım notları için [cite: 17]
    public string ClinicalNotes { get; set; } = string.Empty;

    // Hangi müşteriye ait olduğunu belirten Yabancı Anahtar (Foreign Key)
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    // Bir hayvanın birden fazla randevusu olabilir
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}