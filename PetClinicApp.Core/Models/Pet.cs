namespace PetClinicApp.Core.Models;

/// <summary>
/// Pet entity model.
/// Represents a pet belonging to a client (Foreign Key: ClientId).
/// A pet can have multiple appointments (one-to-many relationship).
/// </summary>
public class Pet
{
    /// <summary>Database primary key (auto increment)</summary>
    public int Id { get; set; }

    /// <summary>Pet's name (e.g. Max, Bella)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Pet's species (e.g. Dog, Cat, Bird)</summary>
    public string Species { get; set; } = string.Empty;

    /// <summary>Pet's age (in years)</summary>
    public int Age { get; set; }

    /// <summary>Pet's gender (Male/Female)</summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Clinical/grooming notes for the pet.
    /// Vet examination results or grooming notes go here.
    /// E.g., "Rabies vaccine administered", "Nails clipped"
    /// </summary>
    public string ClinicalNotes { get; set; } = string.Empty;

    /// <summary>Foreign Key indicating the owner of this pet (ClientId)</summary>
    public int ClientId { get; set; }

    /// <summary>Related client object (Navigation Property for Eager Loading)</summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Collection of appointments for this pet.
    /// One-to-Many relationship: A pet can have multiple appointments.
    /// </summary>
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}