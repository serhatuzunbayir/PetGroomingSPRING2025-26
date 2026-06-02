using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Web.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ClinicService _service = new();

    public IActionResult Index()
    {
        ViewBag.Summary = _service.GetClinicSummary();
        ViewBag.TodaysAppointments = _service.GetTodaysAppointments();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
