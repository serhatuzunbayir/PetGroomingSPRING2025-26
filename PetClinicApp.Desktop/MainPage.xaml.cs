using PetClinicApp.Core.Services;

namespace PetClinicApp.Desktop;

/// <summary>
/// Uygulama açılışında gösterilen Dashboard (Ana Sayfa) sayfası.
/// Klinik özet istatistiklerini ve bugünkü randevuları gösterir.
/// Gereksinim #10: Operational Dashboard — LINQ aggregation ile doldurulur.
/// </summary>
public partial class MainPage : ContentPage
{
    // Tüm iş mantığına erişim için servis katmanı
    private readonly ClinicService _service = new();

    /// <summary>
    /// Sayfa constructor'ı: UI bileşenlerini başlatır.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Sayfa her görünür olduğunda dashboard verilerini yeniler.
    /// Bu sayede başka sekmelerden geri dönüldüğünde veriler güncel kalır.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDashboard(); // Sayfa görünür olduğunda dashboard'ı yenile
    }

    /// <summary>
    /// Dashboard'daki tüm istatistik kartlarını ve bugünkü randevuları yükler.
    /// LINQ aggregation (Sum, Count) ve LINQ filtering (Where, OrderBy) kullanır.
    /// </summary>
    private void LoadDashboard()
    {
        try
        {
            // LINQ Count: Toplam müşteri ve hayvan sayılarını getir
            var clients = _service.GetAllClients();
            var pets = _service.GetAllPets();

            // Kart etiketlerini güncelle
            LblTotalClients.Text = clients.Count.ToString();
            LblTotalPets.Text = pets.Count.ToString();

            // LINQ Aggregation: Toplam kazanç ve özet bilgisi
            var summary = _service.GetClinicSummary();
            LblClinicSummary.Text = summary;

            // LINQ Sum: Ödenen randevuların toplam geliri
            var appointments = _service.GetAllAppointments();
            decimal totalEarnings = 0;
            foreach (var a in appointments)
            {
                // Sadece ödenen randevuların ücretlerini topla
                if (a.IsPaid)
                    totalEarnings += a.ServiceFee;
            }
            LblTotalEarnings.Text = $"${totalEarnings:F2}";

            // LINQ Filtering: Bugünkü randevuları saat sırasına göre getir
            var todaysAppointments = _service.GetTodaysAppointments();

            // Bugün kaç randevu olduğunu göster
            LblTodayCount.Text = todaysAppointments.Count > 0
                ? $"{todaysAppointments.Count} appointment(s) today"
                : "No appointments scheduled for today.";

            // Bugünkü randevular listesini CollectionView'e bağla
            TodayAppointmentsList.ItemsSource = todaysAppointments;
        }
        catch (Exception ex)
        {
            // Hata durumunda crash olmadan mesaj göster
            LblClinicSummary.Text = $"Error loading data: {ex.Message}";
        }
    }

    /// <summary>
    /// "Add New Client" butonuna tıklanınca Clients sekmesine gider.
    /// </summary>
    private async void OnGoToClientsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ClientsPage");
    }

    /// <summary>
    /// "Add New Pet" butonuna tıklanınca Pets sekmesine gider.
    /// </summary>
    private async void OnGoToPetsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PetsPage");
    }

    /// <summary>
    /// "Appointments" butonuna tıklanınca Appointments sekmesine gider.
    /// </summary>
    private async void OnGoToAppointmentsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AppointmentsPage");
    }
}
