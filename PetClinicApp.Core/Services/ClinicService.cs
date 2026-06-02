using PetClinicApp.Core.Models;
using PetClinicApp.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace PetClinicApp.Core.Services;

/// <summary>
/// Core business logic layer of the clinic.
/// Manages Client, Pet, and Appointment operations.
/// LINQ queries, Delegate/Event notifications run through this class.
/// </summary>
public class ClinicService
{
    // ==========================================
    // 1. CLIENT OPERATIONS
    // ==========================================

    /// <summary>
    /// Adds a new client record to the database.
    /// </summary>
    /// <param name="client">The client object to add</param>
    public void AddClient(Client client)
    {
        using var context = new AppDbContext();
        context.Clients.Add(client);
        context.SaveChanges();
    }

    /// <summary>
    /// Gets all client records along with their associated pets.
    /// (LINQ: Eager Loading - join with Include)
    /// </summary>
    /// <returns>Client list including pets</returns>
    public List<Client> GetAllClients()
    {
        using var context = new AppDbContext();
        // Join with Pets table using Include (Eager Loading)
        return context.Clients.Include(c => c.Pets).ToList();
    }

    /// <summary>
    /// Updates an existing client record.
    /// </summary>
    /// <param name="client">The client object to update</param>
    public void UpdateClient(Client client)
    {
        using var context = new AppDbContext();
        context.Clients.Update(client);
        context.SaveChanges();
    }

    /// <summary>
    /// Deletes the client with the given ID from the database.
    /// </summary>
    /// <param name="clientId">ID of the client to delete</param>
    public void DeleteClient(int clientId)
    {
        using var context = new AppDbContext();
        var client = context.Clients.Find(clientId);
        if (client != null)
        {
            context.Clients.Remove(client);
            context.SaveChanges();
        }
    }

    // ==========================================
    // 2. PET OPERATIONS
    // ==========================================

    /// <summary>
    /// Adds a new pet record to the database.
    /// </summary>
    /// <param name="pet">The pet object to add</param>
    public void AddPet(Pet pet)
    {
        using var context = new AppDbContext();
        context.Pets.Add(pet);
        context.SaveChanges();
    }

    /// <summary>
    /// Gets all pets along with their owner client info.
    /// (LINQ: Eager Loading - join with Include)
    /// </summary>
    /// <returns>Pet list including owner info</returns>
    public List<Pet> GetAllPets()
    {
        using var context = new AppDbContext();
        // Bring their owners (Client) while listing pets (Eager Loading)
        return context.Pets.Include(p => p.Client).ToList();
    }

    /// <summary>
    /// Updates an existing pet record.
    /// </summary>
    /// <param name="pet">The pet object to update</param>
    public void UpdatePet(Pet pet)
    {
        using var context = new AppDbContext();
        context.Pets.Update(pet);
        context.SaveChanges();
    }

    /// <summary>
    /// Deletes the pet with the given ID from the database.
    /// </summary>
    /// <param name="petId">ID of the pet to delete</param>
    public void DeletePet(int petId)
    {
        using var context = new AppDbContext();
        var pet = context.Pets.Find(petId);
        if (pet != null)
        {
            context.Pets.Remove(pet);
            context.SaveChanges();
        }
    }

    // ==========================================
    // 3. APPOINTMENT OPERATIONS
    // ==========================================

    /// <summary>
    /// Adds a new appointment record to the database.
    /// </summary>
    /// <param name="appointment">The appointment object to add</param>
    public void AddAppointment(Appointment appointment)
    {
        using var context = new AppDbContext();
        context.Appointments.Add(appointment);
        context.SaveChanges();
    }

    /// <summary>
    /// Gets all appointments along with their associated pet info.
    /// (LINQ: Eager Loading - join with Include)
    /// </summary>
    /// <returns>Appointment list including pet info</returns>
    public List<Appointment> GetAllAppointments()
    {
        using var context = new AppDbContext();
        // Bring which pet it belongs to while listing appointments
        return context.Appointments.Include(a => a.Pet).ToList();
    }

    /// <summary>
    /// Updates an existing appointment record (status, fee etc.).
    /// </summary>
    /// <param name="appointment">The appointment object to update</param>
    public void UpdateAppointment(Appointment appointment)
    {
        using var context = new AppDbContext();
        context.Appointments.Update(appointment);
        context.SaveChanges();
    }

    /// <summary>
    /// Deletes the appointment with the given ID from the database.
    /// </summary>
    /// <param name="appointmentId">ID of the appointment to delete</param>
    public void DeleteAppointment(int appointmentId)
    {
        using var context = new AppDbContext();
        var appointment = context.Appointments.Find(appointmentId);
        if (appointment != null)
        {
            context.Appointments.Remove(appointment);
            context.SaveChanges();
        }
    }

    // ==========================================
    // 4. DELEGATES & EVENTS
    // ==========================================

    /// <summary>
    /// Clinic notification delegate: carries message and related object.
    /// Desktop and Web layers receive notifications via this delegate.
    /// </summary>
    /// <param name="message">The notification message to show</param>
    /// <param name="entity">The processed object (Appointment etc.)</param>
    public delegate void ClinicNotifyHandler(string message, object entity);

    /// <summary>Event triggered when a new appointment is created</summary>
    public event ClinicNotifyHandler? OnAppointmentCreated;

    /// <summary>Event triggered when an appointment is deleted</summary>
    public event ClinicNotifyHandler? OnAppointmentDeleted;

    /// <summary>
    /// Adds an appointment and triggers the OnAppointmentCreated event upon successful save.
    /// (Requirement: Delegate/Event notification)
    /// </summary>
    /// <param name="appointment">The appointment object to add</param>
    public void AddAppointmentWithNotification(Appointment appointment)
    {
        // Save the appointment first
        AddAppointment(appointment);

        // Trigger the delegate event - UI layer listens to this event and shows a notification
        OnAppointmentCreated?.Invoke($"✅ A new appointment has been added for {appointment.AppointmentDate:dd MMM yyyy HH:mm}!", appointment);
    }

    /// <summary>
    /// Deletes the appointment and triggers the OnAppointmentDeleted event upon successful deletion.
    /// (Requirement: Delegate/Event notification)
    /// </summary>
    /// <param name="appointmentId">ID of the appointment to delete</param>
    public void DeleteAppointmentWithNotification(int appointmentId)
    {
        using var context = new AppDbContext();
        var appointment = context.Appointments.Find(appointmentId);

        if (appointment != null)
        {
            // Delete the appointment first
            DeleteAppointment(appointmentId);

            // Trigger the delegate event - UI layer is notified after deletion
            OnAppointmentDeleted?.Invoke("🗑️ Appointment has been successfully removed from the system.", appointment);
        }
    }

    // ==========================================
    // 5. LINQ QUERIES
    // ==========================================

    /// <summary>
    /// Gets all today's appointments; ordered by time.
    /// (LINQ: Filtering + Ordering + ThenInclude/Join)
    /// </summary>
    /// <returns>Today's appointments - along with pet and client info</returns>
    public List<Appointment> GetTodaysAppointments()
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)
                .ThenInclude(p => p!.Client) // Join related tables (ThenInclude)
            .Where(a => a.AppointmentDate.Date == DateTime.Today) // Filtering: only today
            .OrderBy(a => a.AppointmentDate)                      // Ordering: by time
            .ToList();
    }

    /// <summary>
    /// Searches for clients by first or last name using LINQ.
    /// (LINQ: Search + Filtering)
    /// </summary>
    /// <param name="term">Search term (first or last name)</param>
    /// <returns>Client list matching the search term</returns>
    public List<Client> SearchClients(string term)
    {
        // If search term is empty, return all clients
        if (string.IsNullOrWhiteSpace(term)) return GetAllClients();

        using var context = new AppDbContext();
        return context.Clients
            .Where(c => c.FirstName.ToLower().Contains(term.ToLower()) ||
                        c.LastName.ToLower().Contains(term.ToLower())) // First or last name filtering
            .OrderBy(c => c.FirstName)                                   // Order by name
            .ToList();
    }

    /// <summary>
    /// Calculates clinic summary statistics: total earnings and registered pet count.
    /// (LINQ: Aggregation - Sum and Count)
    /// </summary>
    /// <returns>Summary info text</returns>
    public string GetClinicSummary()
    {
        using var context = new AppDbContext();

        // LINQ Sum: Calculate the total amount of only paid appointments
        decimal totalEarnings = context.Appointments.Where(a => a.IsPaid).Sum(a => a.ServiceFee);

        // LINQ Count: Count the total registered pets
        int totalPets = context.Pets.Count();

        return $"There are {totalPets} registered pets in the clinic. Total Earnings: {totalEarnings:C2}";
    }

    /// <summary>
    /// Gets all pets belonging to a specific client, ordered by name.
    /// (LINQ: Filtering + Ordering)
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns>Pet list for that client</returns>
    public List<Pet> GetPetsByClientId(int clientId)
    {
        using var context = new AppDbContext();
        return context.Pets
            .Where(p => p.ClientId == clientId) // Filter by client
            .OrderBy(p => p.Name)               // Order by name
            .ToList();
    }

    /// <summary>
    /// Gets all appointments belonging to a specific client (for web customer panel).
    /// Lists both past and future appointments ordered by date.
    /// (LINQ: Filtering + Ordering + Include/Join)
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns>All appointments for that client - along with pet info</returns>
    public List<Appointment> GetAppointmentsByClientId(int clientId)
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)                              // Join pet info
                .ThenInclude(p => p!.Client)                  // Also bring client info
            .Where(a => a.Pet!.ClientId == clientId)          // Filter by client
            .OrderByDescending(a => a.AppointmentDate)        // Order descending by date
            .ToList();
    }

    /// <summary>
    /// Gets all unpaid (IsPaid = false) and completed appointments.
    /// Used for pending payments on the Dashboard.
    /// (LINQ: Filtering)
    /// </summary>
    /// <returns>Unpaid completed appointment list</returns>
    public List<Appointment> GetUnpaidCompletedAppointments()
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)
            .Where(a => !a.IsPaid && a.Status == AppointmentStatus.Completed) // Filtering
            .OrderBy(a => a.AppointmentDate)
            .ToList();
    }
}