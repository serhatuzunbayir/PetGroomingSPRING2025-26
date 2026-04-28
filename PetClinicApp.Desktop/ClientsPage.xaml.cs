using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Desktop;

public partial class ClientsPage : ContentPage
{
    private readonly ClinicService _service = new();
    private Client? _selectedClient = null;

    public ClientsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadClients();
    }

    // ==========================================
    // DATA LOADING
    // ==========================================

    private void LoadClients()
    {
        try
        {
            var clients = _service.GetAllClients();
            ClientListView.ItemsSource = clients;
            LblRecordCount.Text = $"{clients.Count} records";
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("Error", $"Failed to load clients: {ex.Message}", "OK");
        }
    }

    // ==========================================
    // CRUD OPERATIONS
    // ==========================================

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(EntryFirstName.Text) ||
                string.IsNullOrWhiteSpace(EntryLastName.Text))
            {
                await DisplayAlertAsync("Validation Error", "First Name and Last Name are required.", "OK");
                return;
            }

            var client = new Client
            {
                FirstName = EntryFirstName.Text.Trim(),
                LastName = EntryLastName.Text.Trim(),
                PhoneNumber = EntryPhone.Text?.Trim() ?? string.Empty,
                Email = EntryEmail.Text?.Trim() ?? string.Empty
            };

            _service.AddClient(client);
            LblStatus.Text = "✅ Client saved successfully!";
            LblStatus.TextColor = Color.FromArgb("#2A9D8F");

            ClearForm();
            LoadClients();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to save client: {ex.Message}", "OK");
        }
    }

    private async void OnUpdateClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_selectedClient == null)
            {
                await DisplayAlertAsync("Warning", "Please select a client to update.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EntryFirstName.Text) ||
                string.IsNullOrWhiteSpace(EntryLastName.Text))
            {
                await DisplayAlertAsync("Validation Error", "First Name and Last Name are required.", "OK");
                return;
            }

            _selectedClient.FirstName = EntryFirstName.Text.Trim();
            _selectedClient.LastName = EntryLastName.Text.Trim();
            _selectedClient.PhoneNumber = EntryPhone.Text?.Trim() ?? string.Empty;
            _selectedClient.Email = EntryEmail.Text?.Trim() ?? string.Empty;

            _service.UpdateClient(_selectedClient);
            LblStatus.Text = "✅ Client updated successfully!";
            LblStatus.TextColor = Color.FromArgb("#F4A261");

            ClearForm();
            LoadClients();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to update client: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_selectedClient == null)
            {
                await DisplayAlertAsync("Warning", "Please select a client to delete.", "OK");
                return;
            }

            bool confirm = await DisplayAlertAsync("Confirm Delete",
                $"Are you sure you want to delete {_selectedClient.FirstName} {_selectedClient.LastName}?\n\nThis will also delete all their pets and appointments.",
                "Yes, Delete", "Cancel");

            if (!confirm) return;

            _service.DeleteClient(_selectedClient.Id);
            LblStatus.Text = "🗑️ Client deleted successfully!";
            LblStatus.TextColor = Color.FromArgb("#E76F51");

            ClearForm();
            LoadClients();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to delete client: {ex.Message}", "OK");
        }
    }

    // ==========================================
    // SELECTION & SEARCH
    // ==========================================

    private void OnClientSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Client selected)
        {
            _selectedClient = selected;

            // Fill form fields with selected client data
            EntryFirstName.Text = selected.FirstName;
            EntryLastName.Text = selected.LastName;
            EntryPhone.Text = selected.PhoneNumber;
            EntryEmail.Text = selected.Email;

            // Enable edit/delete buttons
            BtnUpdate.IsEnabled = true;
            BtnDelete.IsEnabled = true;
            BtnSave.IsEnabled = false;

            LblStatus.Text = $"Selected: {selected.FirstName} {selected.LastName}";
            LblStatus.TextColor = Color.FromArgb("#457B9D");
        }
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        PerformSearch();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Search as user types
        PerformSearch();
    }

    private void PerformSearch()
    {
        try
        {
            string term = SearchBarClients.Text ?? string.Empty;
            var results = _service.SearchClients(term);
            ClientListView.ItemsSource = results;
            LblRecordCount.Text = $"{results.Count} records";
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("Error", $"Search failed: {ex.Message}", "OK");
        }
    }

    // ==========================================
    // FORM HELPERS
    // ==========================================

    private void OnClearClicked(object? sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        _selectedClient = null;

        EntryFirstName.Text = string.Empty;
        EntryLastName.Text = string.Empty;
        EntryPhone.Text = string.Empty;
        EntryEmail.Text = string.Empty;

        ClientListView.SelectedItem = null;

        BtnSave.IsEnabled = true;
        BtnUpdate.IsEnabled = false;
        BtnDelete.IsEnabled = false;

        LblStatus.Text = string.Empty;
    }
}
