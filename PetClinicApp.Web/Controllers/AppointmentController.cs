using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;
using PetClinicApp.Web.Models;

namespace PetClinicApp.Web.Controllers;

/// <summary>
/// Web müşteri paneli için randevu işlemlerini yöneten MVC Controller.
/// Müşterilerin randevu almasını, kendi randevularını görüntülemesini
/// ve randevu iptali yapmasını sağlar.
/// </summary>
public class AppointmentController : Controller
{
    // Tüm iş mantığına erişim için servis katmanı
    private readonly ClinicService _service = new();

    /// <summary>
    /// Tüm randevuları listeler — klinik genel randevu takvimi.
    /// (LINQ: GetAllAppointments + GetTodaysAppointments)
    /// GET: /Appointment
    /// </summary>
    public IActionResult Index()
    {
        // Tüm randevuları getir (LINQ eager loading ile)
        var appointments = _service.GetAllAppointments();

        // Bugünkü randevu sayısını ViewBag ile view'a ilet
        ViewBag.TodayCount = _service.GetTodaysAppointments().Count;
        ViewBag.TotalCount = appointments.Count;

        return View(appointments);
    }

    /// <summary>
    /// Randevu alma formunu gösterir.
    /// Müşteri ve hayvan dropdown'larını doldurur.
    /// GET: /Appointment/Book
    /// </summary>
    public IActionResult Book()
    {
        // Form için müşteri listesini hazırla
        var viewModel = new BookAppointmentViewModel
        {
            Clients = _service.GetAllClients(),  // Tüm müşterileri getir
            AppointmentDate = DateTime.Today.AddDays(1) // Varsayılan: yarın
        };

        return View(viewModel);
    }

    /// <summary>
    /// Randevu alma formunu işler ve veritabanına kaydeder.
    /// Delegate/Event mekanizmasıyla bildirim tetiklenir.
    /// POST: /Appointment/Book
    /// </summary>
    /// <param name="viewModel">Formdan gelen randevu verisi</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Book(BookAppointmentViewModel viewModel)
    {
        // Form validasyonu başarısız ise formu yeniden göster
        if (!ModelState.IsValid)
        {
            // Dropdown listelerini yeniden doldur
            viewModel.Clients = _service.GetAllClients();
            viewModel.Pets = _service.GetPetsByClientId(viewModel.SelectedClientId);
            return View(viewModel);
        }

        // Yeni randevu nesnesi oluştur
        var appointment = new Appointment
        {
            PetId = viewModel.SelectedPetId,
            AppointmentDate = viewModel.AppointmentDate,
            Type = viewModel.Type,
            Status = AppointmentStatus.Pending, // Yeni randevular varsayılan olarak beklemede
            ServiceFee = viewModel.ServiceFee,
            IsPaid = false
        };

        // Delegate/Event: Randevu ekle ve OnAppointmentCreated event'ini tetikle
        string notificationMessage = string.Empty;
        _service.OnAppointmentCreated += (msg, _) => notificationMessage = msg;
        _service.AddAppointmentWithNotification(appointment);

        // Bildirim mesajını TempData ile view'a ilet (Redirect sonrası kaybolmaması için)
        TempData["SuccessMessage"] = notificationMessage;

        // Başarılı kayıt sonrası müşterinin randevularına yönlendir
        return RedirectToAction("MyAppointments", new { clientId = viewModel.SelectedClientId });
    }

    /// <summary>
    /// Belirli bir müşterinin randevularını listeler.
    /// Müşteri seçimi dropdown ile yapılır.
    /// (LINQ: GetAppointmentsByClientId — Filtering)
    /// GET: /Appointment/MyAppointments?clientId=5
    /// </summary>
    /// <param name="clientId">Görüntülenecek müşterinin ID'si (opsiyonel)</param>
    public IActionResult MyAppointments(int? clientId)
    {
        // Tüm müşterileri dropdown için getir
        var allClients = _service.GetAllClients();

        var viewModel = new MyAppointmentsViewModel
        {
            AllClients = allClients
        };

        // Eğer bir müşteri seçildiyse, o müşterinin randevularını LINQ ile getir
        if (clientId.HasValue && clientId.Value > 0)
        {
            // LINQ: Müşteriye göre filtrelenmiş randevular
            viewModel.Appointments = _service.GetAppointmentsByClientId(clientId.Value);
            viewModel.SelectedClient = allClients.FirstOrDefault(c => c.Id == clientId.Value);
        }

        return View(viewModel);
    }

    /// <summary>
    /// Randevu iptal işlemi yapar.
    /// Delegate/Event mekanizmasıyla silme bildirimi tetiklenir.
    /// POST: /Appointment/Cancel/5
    /// </summary>
    /// <param name="id">İptal edilecek randevunun ID'si</param>
    /// <param name="clientId">Yönlendirme için müşteri ID'si</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int id, int clientId)
    {
        // Delegate/Event: Randevuyu sil ve OnAppointmentDeleted event'ini tetikle
        string notificationMessage = string.Empty;
        _service.OnAppointmentDeleted += (msg, _) => notificationMessage = msg;
        _service.DeleteAppointmentWithNotification(id);

        // Silme bildirimi TempData ile ilet
        TempData["InfoMessage"] = notificationMessage;

        // Müşterinin randevu listesine geri dön
        return RedirectToAction("MyAppointments", new { clientId });
    }

    /// <summary>
    /// Seçili müşteriye ait hayvanları JSON formatında döndürür.
    /// Book formunda müşteri değiştiğinde AJAX ile çağrılır.
    /// GET: /Appointment/GetPetsByClient?clientId=3
    /// </summary>
    /// <param name="clientId">Müşteri ID'si</param>
    /// <returns>O müşterinin hayvanları (JSON)</returns>
    public JsonResult GetPetsByClient(int clientId)
    {
        // LINQ: Müşteriye göre hayvanları filtrele
        var pets = _service.GetPetsByClientId(clientId)
            .Select(p => new { id = p.Id, name = p.Name, species = p.Species })
            .ToList();

        return Json(pets);
    }
}
