using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Web.Controllers;

/// <summary>
/// MVC Controller that manages pet information for the web customer panel.
/// Allows customers to view their own pets and clinical notes.
/// </summary>
public class PetController : Controller
{
    // Service layer for access to all business logic
    private readonly ClinicService _service = new();

    /// <summary>
    /// Lists all pets.
    /// (LINQ: GetAllPets - Eager Loading including owner info)
    /// GET: /Pet
    /// </summary>
    public IActionResult Index()
    {
        // Get all pets along with client info (LINQ Include)
        var pets = _service.GetAllPets();

        // Pass total pet count to the view via ViewBag
        ViewBag.TotalPets = pets.Count;

        return View(pets);
    }

    /// <summary>
    /// Shows the details page of a specific pet.
    /// Includes clinical notes and appointment history.
    /// (LINQ: GetAppointmentsByClientId - past appointments of the pet)
    /// GET: /Pet/Details/5
    /// </summary>
    /// <param name="id">ID of the pet to view</param>
    public IActionResult Details(int id)
    {
        // Get all pets and find by ID
        var pet = _service.GetAllPets().FirstOrDefault(p => p.Id == id);

        // Redirect to 404 page if pet is not found
        if (pet == null)
            return NotFound();

        // Get all appointments belonging to this pet's owner (LINQ filtering)
        var appointments = _service.GetAppointmentsByClientId(pet.ClientId)
            .Where(a => a.PetId == id) // Only appointments for this pet
            .ToList();

        // Pass appointment history to the view via ViewBag
        ViewBag.Appointments = appointments;
        ViewBag.CompletedCount = appointments.Count(a => a.Status == Core.Models.AppointmentStatus.Completed);
        ViewBag.PendingCount = appointments.Count(a => a.Status == Core.Models.AppointmentStatus.Pending);

        return View(pet);
    }
}
