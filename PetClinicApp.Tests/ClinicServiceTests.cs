using System;
using System.Linq;
using Xunit;
using PetClinicApp.Core.Services;
using PetClinicApp.Core.Models;

namespace PetClinicApp.Tests;

public class ClinicServiceTests
{
    // Test 1: Add Appointment (CRUD - Create)
    [Fact]
    public void AddAppointment_ShouldIncreaseAppointmentCount_WhenValidDataIsProvided()
    {
        // Arrange
        var service = new ClinicService();
        
        // Create a test client and pet to satisfy the Foreign Key constraint
        var testClient = new Client { FirstName = "Test", LastName = "User", PhoneNumber = "5551234567" };
        service.AddClient(testClient);

        var testPet = new Pet { Name = "TestDog", ClientId = testClient.Id };
        service.AddPet(testPet);

        int initialCount = service.GetAllAppointments().Count;
        
        var newAppointment = new Appointment 
        { 
            AppointmentDate = DateTime.Today.AddDays(1), 
            Type = AppointmentType.Veterinary,
            ServiceFee = 500,
            IsPaid = false,
            PetId = testPet.Id // Using the valid PetId
        };

        // Act
        service.AddAppointment(newAppointment);
        int newCount = service.GetAllAppointments().Count;

        // Assert
        Assert.Equal(initialCount + 1, newCount);
    }

    // Test 2: Search Clients (LINQ - Search)
    [Fact]
    public void SearchClients_ShouldReturnMatchingClients_WhenTermExists()
    {
        // Arrange
        var service = new ClinicService();
        var testClient = new Client { FirstName = "John", LastName = "Doe", PhoneNumber = "1112223344" };
        service.AddClient(testClient);
        string searchTerm = "John"; 

        // Act
        var results = service.SearchClients(searchTerm);

        // Assert
        Assert.NotNull(results);
        Assert.Contains(results, c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm)); 
    }

    // Test 3: Get Clinic Summary (LINQ - Aggregation)
    [Fact]
    public void GetClinicSummary_ShouldReturnSummaryString()
    {
        // Arrange
        var service = new ClinicService();

        // Act
        string summary = service.GetClinicSummary();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(summary));
        Assert.Contains("Total Earnings", summary);
    }

    // Test 4: Add Client (CRUD - Create)
    [Fact]
    public void AddClient_ShouldSaveToDatabase()
    {
        // Arrange
        var service = new ClinicService();
        int initialCount = service.GetAllClients().Count;
        
        var newClient = new Client { FirstName = "Alice", LastName = "Smith", PhoneNumber = "5550001122" };
        
        // Act
        service.AddClient(newClient);
        
        // Assert
        Assert.Equal(initialCount + 1, service.GetAllClients().Count);
    }

    // Test 5: Delete Client (CRUD - Delete)
    [Fact]
    public void DeleteClient_ShouldRemoveFromDatabase()
    {
        // Arrange
        var service = new ClinicService();
        var newClient = new Client { FirstName = "ToBeDeleted", LastName = "User", PhoneNumber = "1112223344" };
        service.AddClient(newClient);
        int countAfterAdd = service.GetAllClients().Count;

        // Act
        service.DeleteClient(newClient.Id);
        int countAfterDelete = service.GetAllClients().Count;

        // Assert
        Assert.Equal(countAfterAdd - 1, countAfterDelete);
    }

    // Test 6: Get Pets By Client ID (LINQ - Filtering)
    [Fact]
    public void GetPetsByClientId_ShouldReturnCorrectPets()
    {
        // Arrange
        var service = new ClinicService();
        var client = new Client { FirstName = "Emma", LastName = "Watson", PhoneNumber = "9998887766" };
        service.AddClient(client);

        var pet1 = new Pet { Name = "Fluffy", ClientId = client.Id };
        var pet2 = new Pet { Name = "Cinnamon", ClientId = client.Id };
        service.AddPet(pet1);
        service.AddPet(pet2);

        // Act
        var pets = service.GetPetsByClientId(client.Id);

        // Assert
        Assert.Equal(2, pets.Count); // Ensure the 2 pets we just added are returned
        Assert.All(pets, p => Assert.Equal(client.Id, p.ClientId)); // Ensure they belong to the correct client
    }

    // Test 7: Get Today's Appointments (LINQ - Filtering)
    [Fact]
    public void GetTodaysAppointments_ShouldReturnOnlyToday()
    {
        // Arrange
        var service = new ClinicService();
        var client = new Client { FirstName = "Today", LastName = "Appointment", PhoneNumber = "000111" };
        service.AddClient(client);
        var pet = new Pet { Name = "TodayPet", ClientId = client.Id };
        service.AddPet(pet);

        var appToday = new Appointment { AppointmentDate = DateTime.Today, Type = AppointmentType.Grooming, PetId = pet.Id };
        service.AddAppointment(appToday);

        // Act
        var todaysAppointments = service.GetTodaysAppointments();

        // Assert
        Assert.Contains(todaysAppointments, a => a.Id == appToday.Id);
        Assert.All(todaysAppointments, a => Assert.Equal(DateTime.Today, a.AppointmentDate.Date));
    }

    // Test 8: Get All Pets With Client Data (LINQ - Eager Loading)
    [Fact]
    public void GetAllPets_ShouldIncludeClientData()
    {
        // Arrange
        var service = new ClinicService();
        var client = new Client { FirstName = "Michael", LastName = "Scott", PhoneNumber = "123" };
        service.AddClient(client);
        var pet = new Pet { Name = "Cat", ClientId = client.Id };
        service.AddPet(pet);

        // Act
        var allPets = service.GetAllPets();
        var testPet = allPets.FirstOrDefault(p => p.Id == pet.Id);

        // Assert
        Assert.NotNull(testPet);
        Assert.NotNull(testPet.Client); // Ensure Join operation (Include) works successfully
    }

    // Test 9: Update Appointment Status (CRUD - Update)
    [Fact]
    public void UpdateAppointment_ShouldChangeStatus()
    {
        // Arrange
        var service = new ClinicService();
        var client = new Client { FirstName = "Update", LastName = "User", PhoneNumber = "555" };
        service.AddClient(client);
        var pet = new Pet { Name = "UpdatePet", ClientId = client.Id };
        service.AddPet(pet);

        var app = new Appointment { AppointmentDate = DateTime.Today, Status = AppointmentStatus.Pending, PetId = pet.Id };
        service.AddAppointment(app);

        // Act
        app.Status = AppointmentStatus.Completed; // Update the status
        service.UpdateAppointment(app);

        // Assert
        var updatedApp = service.GetAllAppointments().FirstOrDefault(a => a.Id == app.Id);
        Assert.NotNull(updatedApp);
        Assert.Equal(AppointmentStatus.Completed, updatedApp.Status);
    }

    // Test 10: Delete Appointment (CRUD - Delete)
    [Fact]
    public void DeleteAppointment_ShouldDecreaseCount()
    {
        // Arrange
        var service = new ClinicService();
        var client = new Client { FirstName = "Delete", LastName = "User", PhoneNumber = "12345" };
        service.AddClient(client);
        var pet = new Pet { Name = "DeletePet", ClientId = client.Id };
        service.AddPet(pet);

        var app = new Appointment { AppointmentDate = DateTime.Today, PetId = pet.Id };
        service.AddAppointment(app);

        int countBeforeDelete = service.GetAllAppointments().Count;
        
        // Act
        service.DeleteAppointment(app.Id);
        int countAfterDelete = service.GetAllAppointments().Count;

        // Assert
        Assert.Equal(countBeforeDelete - 1, countAfterDelete);
    }
}