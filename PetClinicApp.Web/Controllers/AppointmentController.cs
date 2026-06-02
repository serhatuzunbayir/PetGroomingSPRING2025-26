using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;
using PetClinicApp.Web.Models;

namespace PetClinicApp.Web.Controllers;

/// <summary>
/// MVC Controller that manages appointment operations for the web customer panel.
/// Allows customers to book appointments, view their own appointments,
/// and cancel appointments.
/// </summary>
public class AppointmentController : Controller
{
    // Service layer for access to all business logic
    private readonly ClinicService _service = new();

    /// <summary>
    /// Lists all appointments - general clinic appointment calendar.
    /// (LINQ: GetAllAppointments + GetTodaysAppointments)
    /// GET: /Appointment
    /// </summary>
    public IActionResult Index()
    {
        // Get all appointments (with LINQ eager loading)
        var appointments = _service.GetAllAppointments();

        // Pass today's appointment count to the view via ViewBag
        ViewBag.TodayCount = _service.GetTodaysAppointments().Count;
        ViewBag.TotalCount = appointments.Count;

        return View(appointments);
    }

    /// <summary>
    /// Shows the appointment booking form.
    /// Populates client and pet dropdowns.
    /// GET: /Appointment/Book
    /// </summary>
    public IActionResult Book()
    {
        // Prepare client list for the form
        var viewModel = new BookAppointmentViewModel
        {
            Clients = _service.GetAllClients(),  // Get all clients
            AppointmentDate = DateTime.Today.AddDays(1) // Default: tomorrow
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processes the appointment booking form and saves to the database.
    /// Triggers notification via Delegate/Event mechanism.
    /// POST: /Appointment/Book
    /// </summary>
    /// <param name="viewModel">Appointment data from the form</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Book(BookAppointmentViewModel viewModel)
    {
        // If form validation fails, show the form again
        if (!ModelState.IsValid)
        {
            // Re-populate dropdown lists
            viewModel.Clients = _service.GetAllClients();
            viewModel.Pets = _service.GetPetsByClientId(viewModel.SelectedClientId);
            return View(viewModel);
        }

        // Create new appointment object
        var appointment = new Appointment
        {
            PetId = viewModel.SelectedPetId,
            AppointmentDate = viewModel.AppointmentDate,
            Type = viewModel.Type,
            Status = AppointmentStatus.Pending, // New appointments are pending by default
            ServiceFee = viewModel.ServiceFee,
            IsPaid = false
        };

        // Delegate/Event: Add appointment and trigger OnAppointmentCreated event
        string notificationMessage = string.Empty;
        _service.OnAppointmentCreated += (msg, _) => notificationMessage = msg;
        _service.AddAppointmentWithNotification(appointment);

        // Pass notification message to view via TempData (so it persists after redirect)
        TempData["SuccessMessage"] = notificationMessage;

        // Redirect to client's appointments after successful booking
        return RedirectToAction("MyAppointments", new { clientId = viewModel.SelectedClientId });
    }

    /// <summary>
    /// Lists appointments for a specific client.
    /// Client selection is done via dropdown.
    /// (LINQ: GetAppointmentsByClientId - Filtering)
    /// GET: /Appointment/MyAppointments?clientId=5
    /// </summary>
    /// <param name="clientId">ID of the client to view (optional)</param>
    public IActionResult MyAppointments(int? clientId)
    {
        // Get all clients for dropdown
        var allClients = _service.GetAllClients();

        var viewModel = new MyAppointmentsViewModel
        {
            AllClients = allClients
        };

        // If a client is selected, get their appointments via LINQ
        if (clientId.HasValue && clientId.Value > 0)
        {
            // LINQ: Appointments filtered by client
            viewModel.Appointments = _service.GetAppointmentsByClientId(clientId.Value);
            viewModel.SelectedClient = allClients.FirstOrDefault(c => c.Id == clientId.Value);
        }

        return View(viewModel);
    }

    /// <summary>
    /// Cancels an appointment.
    /// Triggers deletion notification via Delegate/Event mechanism.
    /// POST: /Appointment/Cancel/5
    /// </summary>
    /// <param name="id">ID of the appointment to cancel</param>
    /// <param name="clientId">Client ID for redirection</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int id, int clientId)
    {
        // Delegate/Event: Delete appointment and trigger OnAppointmentDeleted event
        string notificationMessage = string.Empty;
        _service.OnAppointmentDeleted += (msg, _) => notificationMessage = msg;
        _service.DeleteAppointmentWithNotification(id);

        // Pass deletion notification via TempData
        TempData["InfoMessage"] = notificationMessage;

        // Return to client's appointment list
        return RedirectToAction("MyAppointments", new { clientId });
    }

    /// <summary>
    /// Returns pets belonging to the selected client in JSON format.
    /// Called via AJAX when client changes in Book form.
    /// GET: /Appointment/GetPetsByClient?clientId=3
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns>Pets of that client (JSON)</returns>
    public JsonResult GetPetsByClient(int clientId)
    {
        // LINQ: Filter pets by client
        var pets = _service.GetPetsByClientId(clientId)
            .Select(p => new { id = p.Id, name = p.Name, species = p.Species })
            .ToList();

        return Json(pets);
    }
}
