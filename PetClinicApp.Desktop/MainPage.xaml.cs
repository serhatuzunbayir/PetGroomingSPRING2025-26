using PetClinicApp.Core.Services;

namespace PetClinicApp.Desktop;

/// <summary>
/// Dashboard (Home) page shown on app startup.
/// Displays clinic summary statistics and today's appointments.
/// Requirement #10: Operational Dashboard - populated via LINQ aggregation.
/// </summary>
public partial class MainPage : ContentPage
{
    // Service layer for access to all business logic
    private readonly ClinicService _service = new();

    /// <summary>
    /// Page constructor: Initializes UI components.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Refreshes dashboard data every time the page appears.
    /// Keeps data up-to-date when returning from other tabs.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDashboard(); // Refresh dashboard when page appears
    }

    /// <summary>
    /// Loads all statistic cards and today's appointments on the dashboard.
    /// Uses LINQ aggregation (Sum, Count) and LINQ filtering (Where, OrderBy).
    /// </summary>
    private void LoadDashboard()
    {
        try
        {
            // LINQ Count: Get total client and pet counts
            var clients = _service.GetAllClients();
            var pets = _service.GetAllPets();

            // Update card labels
            LblTotalClients.Text = clients.Count.ToString();
            LblTotalPets.Text = pets.Count.ToString();

            // LINQ Aggregation: Total earnings and summary info
            var summary = _service.GetClinicSummary();
            LblClinicSummary.Text = summary;

            // LINQ Sum: Total earnings of paid appointments
            var appointments = _service.GetAllAppointments();
            decimal totalEarnings = 0;
            foreach (var a in appointments)
            {
                // Only sum fees of paid appointments
                if (a.IsPaid)
                    totalEarnings += a.ServiceFee;
            }
            LblTotalEarnings.Text = $"${totalEarnings:F2}";

            // LINQ Filtering: Get today's appointments ordered by time
            var todaysAppointments = _service.GetTodaysAppointments();

            // Show how many appointments there are today
            LblTodayCount.Text = todaysAppointments.Count > 0
                ? $"{todaysAppointments.Count} appointment(s) today"
                : "No appointments scheduled for today.";

            // Bind today's appointments list to CollectionView
            TodayAppointmentsList.ItemsSource = todaysAppointments;
        }
        catch (Exception ex)
        {
            // Show message without crashing on error
            LblClinicSummary.Text = $"Error loading data: {ex.Message}";
        }
    }

    /// <summary>
    /// Navigates to Clients tab when "Add New Client" button is clicked.
    /// </summary>
    private async void OnGoToClientsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ClientsPage");
    }

    /// <summary>
    /// Navigates to Pets tab when "Add New Pet" button is clicked.
    /// </summary>
    private async void OnGoToPetsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PetsPage");
    }

    /// <summary>
    /// Navigates to Appointments tab when "Appointments" button is clicked.
    /// </summary>
    private async void OnGoToAppointmentsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AppointmentsPage");
    }
}
