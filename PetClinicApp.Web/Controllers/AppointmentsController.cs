using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Web.Controllers;

public class AppointmentsController : Controller
{
    private readonly ClinicService _service = new();

    public AppointmentsController()
    {
        _service.OnAppointmentCreated += (msg, _) => TempData["Notification"] = msg;
        _service.OnAppointmentDeleted += (msg, _) => TempData["Notification"] = msg;
    }

    public IActionResult Index()
    {
        var appointments = _service.GetAllAppointments();
        return View(appointments);
    }

    public IActionResult Create()
    {
        ViewBag.Pets = _service.GetAllPets();
        return View(new Appointment { AppointmentDate = DateTime.Now });
    }

    [HttpPost]
    public IActionResult Create(Appointment appointment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Pets = _service.GetAllPets();
            return View(appointment);
        }
        _service.AddAppointmentWithNotification(appointment);
        TempData["Success"] = TempData["Notification"]?.ToString() ?? "Appointment added.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var appointments = _service.GetAllAppointments();
        var appointment = appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null) return NotFound();
        ViewBag.Pets = _service.GetAllPets();
        return View(appointment);
    }

    [HttpPost]
    public IActionResult Edit(Appointment appointment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Pets = _service.GetAllPets();
            return View(appointment);
        }
        _service.UpdateAppointment(appointment);
        TempData["Success"] = "Appointment updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        _service.DeleteAppointmentWithNotification(id);
        TempData["Success"] = TempData["Notification"]?.ToString() ?? "Appointment deleted.";
        return RedirectToAction(nameof(Index));
    }
}
