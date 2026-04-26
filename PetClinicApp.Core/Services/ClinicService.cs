using PetClinicApp.Core.Models;
using PetClinicApp.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace PetClinicApp.Core.Services;

public class ClinicService
{
    // ==========================================
    // 1. MÜŞTERİ (CLIENT) İŞLEMLERİ
    // ==========================================
    public void AddClient(Client client)
    {
        using var context = new AppDbContext();
        context.Clients.Add(client);
        context.SaveChanges();
    }

    public List<Client> GetAllClients()
    {
        using var context = new AppDbContext();
        return context.Clients.Include(c => c.Pets).ToList();
    }

    public void UpdateClient(Client client)
    {
        using var context = new AppDbContext();
        context.Clients.Update(client);
        context.SaveChanges();
    }

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
    // 2. EVCİL HAYVAN (PET) İŞLEMLERİ
    // ==========================================
    public void AddPet(Pet pet)
    {
        using var context = new AppDbContext();
        context.Pets.Add(pet);
        context.SaveChanges();
    }

    public List<Pet> GetAllPets()
    {
        using var context = new AppDbContext();
        // Hayvanları listelerken sahiplerini (Client) de getir
        return context.Pets.Include(p => p.Client).ToList();
    }

    public void UpdatePet(Pet pet)
    {
        using var context = new AppDbContext();
        context.Pets.Update(pet);
        context.SaveChanges();
    }

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
    // 3. RANDEVU (APPOINTMENT) İŞLEMLERİ
    // ==========================================
    public void AddAppointment(Appointment appointment)
    {
        using var context = new AppDbContext();
        context.Appointments.Add(appointment);
        context.SaveChanges();
    }

    public List<Appointment> GetAllAppointments()
    {
        using var context = new AppDbContext();
        // Randevuları listelerken hangi hayvana (Pet) ait olduğunu da getir
        return context.Appointments.Include(a => a.Pet).ToList();
    }

    public void UpdateAppointment(Appointment appointment)
    {
        using var context = new AppDbContext();
        context.Appointments.Update(appointment);
        context.SaveChanges();
    }

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

    // Delegate definition
    public delegate void ClinicNotifyHandler(string message, object entity);

    // Two separate events for new record creation and deletion.
    public event ClinicNotifyHandler OnAppointmentCreated;
    public event ClinicNotifyHandler OnAppointmentDeleted;

    /// Adds an appointment and triggers an event if successful.
    public void AddAppointmentWithNotification(Appointment appointment)
    {
        AddAppointment(appointment); 
        
        // Event triggering (Requirement: Notifications)
        OnAppointmentCreated?.Invoke($" A new appointment has been added for {appointment.AppointmentDate}!", appointment);
    }

    /// Deletes the appointment and triggers an event if successful.
    public void DeleteAppointmentWithNotification(int appointmentId)
    {
        using var context = new AppDbContext();
        var appointment = context.Appointments.Find(appointmentId);
        
        if (appointment != null)
        {
            DeleteAppointment(appointmentId); 
            
            // Event triggering (Requirement: Triggering updates)
            OnAppointmentDeleted?.Invoke(" Appointment has been successfully removed from the system.", appointment);
        }
    }

    // ==========================================
    // 5. LINQ QUERIES 
    // ==========================================

    /// Filters today's appointments and orders them by date.
    /// (Requirement: LINQ Filtering & Ordering)
    public List<Appointment> GetTodaysAppointments()
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)
                .ThenInclude(p => p.Client) // Include related tables (Join)
            .Where(a => a.AppointmentDate.Date == DateTime.Today) // Filtering
            .OrderBy(a => a.AppointmentDate) // Ordering
            .ToList();
    }

    /// Searches by client first name or last name.
    /// (Requirement: LINQ Search functionality)
    public List<Client> SearchClients(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return GetAllClients();

        using var context = new AppDbContext();
        return context.Clients
            .Where(c => c.FirstName.ToLower().Contains(term.ToLower()) || 
                        c.LastName.ToLower().Contains(term.ToLower())) // Filtering
            .OrderBy(c => c.FirstName) // Ordering
            .ToList();
    }

    /// Calculates summary data for the clinic (Total earnings and number of pets).
    /// (Requirement: LINQ Aggregating)
    public string GetClinicSummary()
    {
        using var context = new AppDbContext();
        
        // LINQ Sum and Count operations
        decimal totalEarnings = context.Appointments.Where(a => a.IsPaid).Sum(a => a.ServiceFee);
        int totalPets = context.Pets.Count();

        return $"There are {totalPets} registered pets in the clinic. Total Earnings: {totalEarnings:C2}";
    }

    /// Retrieves all pets belonging to a specific client.
    public List<Pet> GetPetsByClientId(int clientId)
    {
        using var context = new AppDbContext();
        return context.Pets
            .Where(p => p.ClientId == clientId)
            .OrderBy(p => p.Name)
            .ToList();
    }
    }