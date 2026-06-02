using PetClinicApp.Core.Models;
using PetClinicApp.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace PetClinicApp.Core.Services;

/// <summary>
/// Kliniğin temel iş mantığı katmanı.
/// Müşteri, Evcil Hayvan ve Randevu işlemlerini yönetir.
/// LINQ sorguları, Delegate/Event bildirimleri bu sınıf üzerinden çalışır.
/// </summary>
public class ClinicService
{
    // ==========================================
    // 1. MÜŞTERİ (CLIENT) İŞLEMLERİ
    // ==========================================

    /// <summary>
    /// Yeni bir müşteri kaydını veritabanına ekler.
    /// </summary>
    /// <param name="client">Eklenecek müşteri nesnesi</param>
    public void AddClient(Client client)
    {
        using var context = new AppDbContext();
        context.Clients.Add(client);
        context.SaveChanges();
    }

    /// <summary>
    /// Tüm müşteri kayıtlarını ilişkili evcil hayvanlarıyla birlikte getirir.
    /// (LINQ: Eager Loading — Include ile join)
    /// </summary>
    /// <returns>Evcil hayvanları dahil müşteri listesi</returns>
    public List<Client> GetAllClients()
    {
        using var context = new AppDbContext();
        // Include ile Pets tablosuyla join yapılıyor (Eager Loading)
        return context.Clients.Include(c => c.Pets).ToList();
    }

    /// <summary>
    /// Mevcut bir müşteri kaydını günceller.
    /// </summary>
    /// <param name="client">Güncellenecek müşteri nesnesi</param>
    public void UpdateClient(Client client)
    {
        using var context = new AppDbContext();
        context.Clients.Update(client);
        context.SaveChanges();
    }

    /// <summary>
    /// Verilen ID'ye ait müşteriyi veritabanından siler.
    /// </summary>
    /// <param name="clientId">Silinecek müşterinin ID'si</param>
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

    /// <summary>
    /// Yeni bir evcil hayvan kaydını veritabanına ekler.
    /// </summary>
    /// <param name="pet">Eklenecek evcil hayvan nesnesi</param>
    public void AddPet(Pet pet)
    {
        using var context = new AppDbContext();
        context.Pets.Add(pet);
        context.SaveChanges();
    }

    /// <summary>
    /// Tüm evcil hayvanları sahip müşteri bilgisiyle birlikte getirir.
    /// (LINQ: Eager Loading — Include ile join)
    /// </summary>
    /// <returns>Sahip bilgisi dahil hayvan listesi</returns>
    public List<Pet> GetAllPets()
    {
        using var context = new AppDbContext();
        // Hayvanları listelerken sahiplerini (Client) de getir (Eager Loading)
        return context.Pets.Include(p => p.Client).ToList();
    }

    /// <summary>
    /// Mevcut bir evcil hayvan kaydını günceller.
    /// </summary>
    /// <param name="pet">Güncellenecek evcil hayvan nesnesi</param>
    public void UpdatePet(Pet pet)
    {
        using var context = new AppDbContext();
        context.Pets.Update(pet);
        context.SaveChanges();
    }

    /// <summary>
    /// Verilen ID'ye ait evcil hayvanı veritabanından siler.
    /// </summary>
    /// <param name="petId">Silinecek hayvanın ID'si</param>
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

    /// <summary>
    /// Yeni bir randevu kaydını veritabanına ekler.
    /// </summary>
    /// <param name="appointment">Eklenecek randevu nesnesi</param>
    public void AddAppointment(Appointment appointment)
    {
        using var context = new AppDbContext();
        context.Appointments.Add(appointment);
        context.SaveChanges();
    }

    /// <summary>
    /// Tüm randevuları ilişkili hayvan bilgisiyle birlikte getirir.
    /// (LINQ: Eager Loading — Include ile join)
    /// </summary>
    /// <returns>Hayvan bilgisi dahil randevu listesi</returns>
    public List<Appointment> GetAllAppointments()
    {
        using var context = new AppDbContext();
        // Randevuları listelerken hangi hayvana (Pet) ait olduğunu da getir
        return context.Appointments.Include(a => a.Pet).ToList();
    }

    /// <summary>
    /// Mevcut bir randevu kaydını günceller (durum, ücret vb.).
    /// </summary>
    /// <param name="appointment">Güncellenecek randevu nesnesi</param>
    public void UpdateAppointment(Appointment appointment)
    {
        using var context = new AppDbContext();
        context.Appointments.Update(appointment);
        context.SaveChanges();
    }

    /// <summary>
    /// Verilen ID'ye ait randevuyu veritabanından siler.
    /// </summary>
    /// <param name="appointmentId">Silinecek randevunun ID'si</param>
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
    /// Klinik bildirim delegate'i: mesaj ve ilgili nesneyi taşır.
    /// Desktop ve Web katmanları bu delegate aracılığıyla bildirim alır.
    /// </summary>
    /// <param name="message">Gösterilecek bildirim mesajı</param>
    /// <param name="entity">İşlem yapılan nesne (Appointment vb.)</param>
    public delegate void ClinicNotifyHandler(string message, object entity);

    /// <summary>Yeni randevu oluşturulduğunda tetiklenen event</summary>
    public event ClinicNotifyHandler? OnAppointmentCreated;

    /// <summary>Randevu silindiğinde tetiklenen event</summary>
    public event ClinicNotifyHandler? OnAppointmentDeleted;

    /// <summary>
    /// Randevu ekler ve başarıyla kaydedilince OnAppointmentCreated event'ini tetikler.
    /// (Gereksinim: Delegate/Event bildirimi)
    /// </summary>
    /// <param name="appointment">Eklenecek randevu nesnesi</param>
    public void AddAppointmentWithNotification(Appointment appointment)
    {
        // Önce randevuyu kaydet
        AddAppointment(appointment);

        // Delegate event'ini tetikle — UI katmanı bu event'i dinler ve bildirim gösterir
        OnAppointmentCreated?.Invoke($"✅ A new appointment has been added for {appointment.AppointmentDate:dd MMM yyyy HH:mm}!", appointment);
    }

    /// <summary>
    /// Randevuyu siler ve başarıyla silinince OnAppointmentDeleted event'ini tetikler.
    /// (Gereksinim: Delegate/Event bildirimi)
    /// </summary>
    /// <param name="appointmentId">Silinecek randevunun ID'si</param>
    public void DeleteAppointmentWithNotification(int appointmentId)
    {
        using var context = new AppDbContext();
        var appointment = context.Appointments.Find(appointmentId);

        if (appointment != null)
        {
            // Önce randevuyu sil
            DeleteAppointment(appointmentId);

            // Delegate event'ini tetikle — silme işlemi sonrası UI katmanı bilgilendirilir
            OnAppointmentDeleted?.Invoke("🗑️ Appointment has been successfully removed from the system.", appointment);
        }
    }

    // ==========================================
    // 5. LINQ QUERIES
    // ==========================================

    /// <summary>
    /// Bugünkü tüm randevuları getirir; saat sırasına göre sıralar.
    /// (LINQ: Filtering + Ordering + ThenInclude/Join)
    /// </summary>
    /// <returns>Bugünkü randevular — hayvan ve müşteri bilgisiyle birlikte</returns>
    public List<Appointment> GetTodaysAppointments()
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)
                .ThenInclude(p => p!.Client) // İlişkili tabloları join et (ThenInclude)
            .Where(a => a.AppointmentDate.Date == DateTime.Today) // Filtreleme: sadece bugün
            .OrderBy(a => a.AppointmentDate)                      // Sıralama: saate göre
            .ToList();
    }

    /// <summary>
    /// Müşteri adı veya soyadına göre LINQ ile arama yapar.
    /// (LINQ: Search + Filtering)
    /// </summary>
    /// <param name="term">Aranacak metin (ad veya soyad)</param>
    /// <returns>Arama terimiyle eşleşen müşteri listesi</returns>
    public List<Client> SearchClients(string term)
    {
        // Arama terimi boşsa tüm müşterileri getir
        if (string.IsNullOrWhiteSpace(term)) return GetAllClients();

        using var context = new AppDbContext();
        return context.Clients
            .Where(c => c.FirstName.ToLower().Contains(term.ToLower()) ||
                        c.LastName.ToLower().Contains(term.ToLower())) // Ad veya soyad filtreleme
            .OrderBy(c => c.FirstName)                                   // İsme göre sıralama
            .ToList();
    }

    /// <summary>
    /// Klinik özet istatistiklerini hesaplar: toplam kazanç ve kayıtlı hayvan sayısı.
    /// (LINQ: Aggregation — Sum ve Count)
    /// </summary>
    /// <returns>Özet bilgi metni</returns>
    public string GetClinicSummary()
    {
        using var context = new AppDbContext();

        // LINQ Sum: Sadece ödenen randevuların toplam tutarını hesapla
        decimal totalEarnings = context.Appointments.Where(a => a.IsPaid).Sum(a => a.ServiceFee);

        // LINQ Count: Kayıtlı toplam hayvan sayısını say
        int totalPets = context.Pets.Count();

        return $"There are {totalPets} registered pets in the clinic. Total Earnings: {totalEarnings:C2}";
    }

    /// <summary>
    /// Belirli bir müşteriye ait tüm evcil hayvanları isim sırasına göre getirir.
    /// (LINQ: Filtering + Ordering)
    /// </summary>
    /// <param name="clientId">Müşteri ID'si</param>
    /// <returns>O müşteriye ait hayvan listesi</returns>
    public List<Pet> GetPetsByClientId(int clientId)
    {
        using var context = new AppDbContext();
        return context.Pets
            .Where(p => p.ClientId == clientId) // Müşteriye göre filtreleme
            .OrderBy(p => p.Name)               // İsme göre sıralama
            .ToList();
    }

    /// <summary>
    /// Belirli bir müşteriye ait tüm randevuları getirir (web müşteri paneli için).
    /// Hem geçmiş hem de gelecek randevuları tarih sırasına göre listeler.
    /// (LINQ: Filtering + Ordering + Include/Join)
    /// </summary>
    /// <param name="clientId">Müşteri ID'si</param>
    /// <returns>O müşterinin tüm randevuları — hayvan bilgisiyle birlikte</returns>
    public List<Appointment> GetAppointmentsByClientId(int clientId)
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)                              // Hayvan bilgisini join et
                .ThenInclude(p => p!.Client)                  // Müşteri bilgisini de getir
            .Where(a => a.Pet!.ClientId == clientId)          // Müşteriye göre filtrele
            .OrderByDescending(a => a.AppointmentDate)        // Tarihe göre azalan sıralama
            .ToList();
    }

    /// <summary>
    /// Tüm ödenmemiş (IsPaid = false) ve tamamlanmış randevuları getirir.
    /// Dashboard'da bekleyen ödemeler için kullanılır.
    /// (LINQ: Filtering)
    /// </summary>
    /// <returns>Ödenmemiş tamamlanmış randevu listesi</returns>
    public List<Appointment> GetUnpaidCompletedAppointments()
    {
        using var context = new AppDbContext();
        return context.Appointments
            .Include(a => a.Pet)
            .Where(a => !a.IsPaid && a.Status == AppointmentStatus.Completed) // Filtreleme
            .OrderBy(a => a.AppointmentDate)
            .ToList();
    }
}