using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Desktop;

public partial class AppointmentsPage : ContentPage
{
    private readonly ClinicService _service = new();
    private Appointment? _selectedAppointment = null;
    private List<Pet> _pets = new();

    public AppointmentsPage()
    {
        InitializeComponent();
        _service.OnAppointmentCreated += OnAppointmentNotification;
        _service.OnAppointmentDeleted += OnAppointmentNotification;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadPets();
        LoadAppointments();
    }

    private async void OnAppointmentNotification(string message, object entity)
    {
        await DisplayAlert("Notification", message, "OK");
    }

    private void LoadPets()
    {
        _pets = _service.GetAllPets();
        PickerPet.ItemsSource = _pets;
    }

    private void LoadAppointments()
    {
        var appointments = _service.GetAllAppointments();
        AppointmentListView.ItemsSource = appointments;
        LblRecordCount.Text = $"{appointments.Count} records";
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (PickerPet.SelectedIndex < 0)
        {
            await DisplayAlert("Warning", "Please select a pet.", "OK");
            return;
        }
        if (PickerType.SelectedIndex < 0)
        {
            await DisplayAlert("Warning", "Please select an appointment type.", "OK");
            return;
        }

        decimal fee = 0;
        if (!string.IsNullOrWhiteSpace(EntryFee.Text) &&
            (!decimal.TryParse(EntryFee.Text, out fee) || fee < 0))
        {
            await DisplayAlert("Warning", "Please enter a valid fee.", "OK");
            return;
        }

        var pet = _pets[PickerPet.SelectedIndex];
        var date = (PickerDate.Date ?? DateTime.Today).Add(PickerTime.Time ?? TimeSpan.Zero);
        var type = PickerType.SelectedItem.ToString() == "Veterinary"
            ? AppointmentType.Veterinary
            : AppointmentType.Grooming;

        var appointment = new Appointment
        {
            PetId = pet.Id,
            AppointmentDate = date,
            Type = type,
            Status = AppointmentStatus.Pending,
            ServiceFee = fee,
            IsPaid = CheckPaid.IsChecked
        };

        try
        {
            _service.AddAppointmentWithNotification(appointment);
            LblStatus.Text = "Appointment saved.";
            ClearForm();
            LoadAppointments();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnUpdateClicked(object? sender, EventArgs e)
    {
        if (_selectedAppointment == null)
        {
            await DisplayAlert("Warning", "Please select an appointment.", "OK");
            return;
        }
        if (PickerPet.SelectedIndex < 0 || PickerType.SelectedIndex < 0)
        {
            await DisplayAlert("Warning", "Please fill in all fields.", "OK");
            return;
        }

        decimal fee = 0;
        if (!string.IsNullOrWhiteSpace(EntryFee.Text) &&
            (!decimal.TryParse(EntryFee.Text, out fee) || fee < 0))
        {
            await DisplayAlert("Warning", "Please enter a valid fee.", "OK");
            return;
        }

        var pet = _pets[PickerPet.SelectedIndex];

        _selectedAppointment.PetId = pet.Id;
        _selectedAppointment.AppointmentDate = (PickerDate.Date ?? DateTime.Today).Add(PickerTime.Time ?? TimeSpan.Zero);
        _selectedAppointment.Type = PickerType.SelectedItem.ToString() == "Veterinary"
            ? AppointmentType.Veterinary
            : AppointmentType.Grooming;
        _selectedAppointment.ServiceFee = fee;
        _selectedAppointment.IsPaid = CheckPaid.IsChecked;

        if (PickerStatus.SelectedIndex >= 0)
        {
            string s = PickerStatus.SelectedItem.ToString()!;
            if (s == "Pending") _selectedAppointment.Status = AppointmentStatus.Pending;
            else if (s == "Completed") _selectedAppointment.Status = AppointmentStatus.Completed;
            else if (s == "Cancelled") _selectedAppointment.Status = AppointmentStatus.Cancelled;
        }

        try
        {
            _service.UpdateAppointment(_selectedAppointment);
            LblStatus.Text = "Appointment updated.";
            ClearForm();
            LoadAppointments();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_selectedAppointment == null)
        {
            await DisplayAlert("Warning", "Please select an appointment.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirm", "Delete this appointment?", "Yes", "Cancel");
        if (!confirm) return;

        try
        {
            _service.DeleteAppointmentWithNotification(_selectedAppointment.Id);
            LblStatus.Text = "Appointment deleted.";
            ClearForm();
            LoadAppointments();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnAppointmentSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Appointment selected) return;

        _selectedAppointment = selected;

        int petIndex = _pets.FindIndex(p => p.Id == selected.PetId);
        if (petIndex >= 0) PickerPet.SelectedIndex = petIndex;

        PickerDate.Date = selected.AppointmentDate.Date;
        PickerTime.Time = selected.AppointmentDate.TimeOfDay;
        PickerType.SelectedIndex = selected.Type == AppointmentType.Veterinary ? 0 : 1;

        if (selected.Status == AppointmentStatus.Pending) PickerStatus.SelectedIndex = 0;
        else if (selected.Status == AppointmentStatus.Completed) PickerStatus.SelectedIndex = 1;
        else if (selected.Status == AppointmentStatus.Cancelled) PickerStatus.SelectedIndex = 2;

        EntryFee.Text = selected.ServiceFee.ToString();
        CheckPaid.IsChecked = selected.IsPaid;

        BtnUpdate.IsEnabled = true;
        BtnDelete.IsEnabled = true;
        BtnSave.IsEnabled = false;
    }

    private void OnShowTodaysClicked(object? sender, EventArgs e)
    {
        var todays = _service.GetTodaysAppointments();
        AppointmentListView.ItemsSource = todays;
        LblRecordCount.Text = $"{todays.Count} records (Today)";
    }

    private void OnShowAllClicked(object? sender, EventArgs e)
    {
        LoadAppointments();
    }

    private void OnFilterTypeChanged(object? sender, EventArgs e)
    {
        if (PickerFilterType.SelectedIndex <= 0)
        {
            LoadAppointments();
            return;
        }

        string selectedType = PickerFilterType.SelectedItem.ToString()!;
        var filtered = _service.GetAllAppointments()
            .Where(a => a.Type.ToString() == selectedType)
            .ToList();

        AppointmentListView.ItemsSource = filtered;
        LblRecordCount.Text = $"{filtered.Count} records ({selectedType})";
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        _selectedAppointment = null;
        PickerPet.SelectedIndex = -1;
        PickerDate.Date = DateTime.Today;
        PickerTime.Time = TimeSpan.FromHours(9);
        PickerType.SelectedIndex = -1;
        PickerStatus.SelectedIndex = -1;
        EntryFee.Text = string.Empty;
        CheckPaid.IsChecked = false;
        AppointmentListView.SelectedItem = null;
        BtnSave.IsEnabled = true;
        BtnUpdate.IsEnabled = false;
        BtnDelete.IsEnabled = false;
        LblStatus.Text = string.Empty;
    }
}
