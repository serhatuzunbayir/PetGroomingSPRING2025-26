namespace PetClinicApp.Core.Models;
public class Client
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // bir müşterinin birden fazla evcil hayvanı olabilir (one to many ilişkisi)
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();    
}