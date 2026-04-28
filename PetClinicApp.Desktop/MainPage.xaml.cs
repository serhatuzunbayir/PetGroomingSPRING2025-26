using PetClinicApp.Core.Services;

namespace PetClinicApp.Desktop;

public partial class MainPage : ContentPage
{
    private readonly ClinicService _service = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        try
        {
            var clients = _service.GetAllClients();
            var pets = _service.GetAllPets();

            LblTotalClients.Text = clients.Count.ToString();
            LblTotalPets.Text = pets.Count.ToString();

            // Calculate total earnings from paid appointments
            var summary = _service.GetClinicSummary();
            LblClinicSummary.Text = summary;

            // Extract earnings from appointments
            var appointments = _service.GetAllAppointments();
            decimal totalEarnings = 0;
            foreach (var a in appointments)
            {
                if (a.IsPaid)
                    totalEarnings += a.ServiceFee;
            }
            LblTotalEarnings.Text = $"${totalEarnings:F2}";
        }
        catch (Exception ex)
        {
            LblClinicSummary.Text = $"Error loading data: {ex.Message}";
        }
    }

    private async void OnGoToClientsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ClientsPage");
    }

    private async void OnGoToPetsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PetsPage");
    }
}
