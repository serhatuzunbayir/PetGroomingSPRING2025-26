namespace PetClinicApp.Core.Models;

/// <summary>
/// Client (pet owner) entity model.
/// A client can have multiple pets (one-to-many relationship).
/// </summary>
public class Client
{
    /// <summary>Database primary key (auto increment)</summary>
    public int Id { get; set; }

    /// <summary>Client's first name</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Client's last name</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Client's phone number (for contact)</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Client's email address</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Collection of pets belonging to the client.
    /// One-to-Many relationship: A client can have multiple pets.
    /// </summary>
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}