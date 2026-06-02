using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Web.Controllers;

/// <summary>
/// Web müşteri paneli için evcil hayvan bilgilerini yöneten MVC Controller.
/// Müşterilerin kendi hayvanlarını ve klinik notlarını görüntülemesini sağlar.
/// </summary>
public class PetController : Controller
{
    // Tüm iş mantığına erişim için servis katmanı
    private readonly ClinicService _service = new();

    /// <summary>
    /// Tüm evcil hayvanları listeler.
    /// (LINQ: GetAllPets — Eager Loading ile sahip bilgisi dahil)
    /// GET: /Pet
    /// </summary>
    public IActionResult Index()
    {
        // Tüm hayvanları müşteri bilgisiyle birlikte getir (LINQ Include)
        var pets = _service.GetAllPets();

        // Toplam hayvan sayısını ViewBag ile view'a ilet
        ViewBag.TotalPets = pets.Count;

        return View(pets);
    }

    /// <summary>
    /// Belirli bir evcil hayvanın detay sayfasını gösterir.
    /// Klinik notları ve randevu geçmişini içerir.
    /// (LINQ: GetAppointmentsByClientId — hayvanın geçmiş randevuları)
    /// GET: /Pet/Details/5
    /// </summary>
    /// <param name="id">Görüntülenecek hayvanın ID'si</param>
    public IActionResult Details(int id)
    {
        // Tüm hayvanları getir ve ID ile bul
        var pet = _service.GetAllPets().FirstOrDefault(p => p.Id == id);

        // Hayvan bulunamadıysa 404 sayfasına yönlendir
        if (pet == null)
            return NotFound();

        // Bu hayvanın sahibine ait tüm randevuları getir (LINQ filtreleme)
        var appointments = _service.GetAppointmentsByClientId(pet.ClientId)
            .Where(a => a.PetId == id) // Sadece bu hayvana ait randevular
            .ToList();

        // Randevu geçmişini ViewBag ile view'a ilet
        ViewBag.Appointments = appointments;
        ViewBag.CompletedCount = appointments.Count(a => a.Status == Core.Models.AppointmentStatus.Completed);
        ViewBag.PendingCount = appointments.Count(a => a.Status == Core.Models.AppointmentStatus.Pending);

        return View(pet);
    }
}
