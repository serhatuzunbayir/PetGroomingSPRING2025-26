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
}