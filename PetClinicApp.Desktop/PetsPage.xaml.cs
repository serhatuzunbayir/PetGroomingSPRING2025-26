using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Desktop;

/// <summary>
/// Helper class to display client name in Picker controls.
/// </summary>
public class ClientDisplayItem
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public partial class PetsPage : ContentPage
{
    private readonly ClinicService _service = new();
    private Pet? _selectedPet = null;
    private List<ClientDisplayItem> _clientItems = new();

    public PetsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadOwners();
        LoadPets();
    }

    // ==========================================
    // DATA LOADING
    // ==========================================

    private void LoadOwners()
    {
        try
        {
            var clients = _service.GetAllClients();
            _clientItems = clients.Select(c => new ClientDisplayItem
            {
                Id = c.Id,
                DisplayName = $"{c.FirstName} {c.LastName}"
            }).ToList();

            PickerOwner.ItemsSource = _clientItems;

            // Filter picker: add "All Owners" option
            var filterItems = new List<ClientDisplayItem>
            {
                new ClientDisplayItem { Id = 0, DisplayName = "All Owners" }
            };
            filterItems.AddRange(_clientItems);
            PickerFilterOwner.ItemsSource = filterItems;
            PickerFilterOwner.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("Error", $"Failed to load owners: {ex.Message}", "OK");
        }
    }

    private void LoadPets()
    {
        try
        {
            var pets = _service.GetAllPets();
            PetListView.ItemsSource = pets;
            LblRecordCount.Text = $"{pets.Count} records";
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("Error", $"Failed to load pets: {ex.Message}", "OK");
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
            if (PickerOwner.SelectedIndex < 0)
            {
                await DisplayAlertAsync("Validation Error", "Please select an owner.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EntryPetName.Text))
            {
                await DisplayAlertAsync("Validation Error", "Pet name is required.", "OK");
                return;
            }

            if (PickerSpecies.SelectedIndex < 0)
            {
                await DisplayAlertAsync("Validation Error", "Please select a species.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EntryAge.Text) || !int.TryParse(EntryAge.Text, out int age) || age < 0)
            {
                await DisplayAlertAsync("Validation Error", "Please enter a valid age.", "OK");
                return;
            }

            if (PickerGender.SelectedIndex < 0)
            {
                await DisplayAlertAsync("Validation Error", "Please select a gender.", "OK");
                return;
            }

            var selectedOwner = _clientItems[PickerOwner.SelectedIndex];

            var pet = new Pet
            {
                Name = EntryPetName.Text.Trim(),
                Species = PickerSpecies.SelectedItem?.ToString() ?? string.Empty,
                Age = age,
                Gender = PickerGender.SelectedItem?.ToString() ?? string.Empty,
                ClinicalNotes = EditorNotes.Text?.Trim() ?? string.Empty,
                ClientId = selectedOwner.Id
            };

            _service.AddPet(pet);
            LblStatus.Text = "✅ Pet saved successfully!";
            LblStatus.TextColor = Color.FromArgb("#2A9D8F");

            ClearForm();
            LoadPets();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to save pet: {ex.Message}", "OK");
        }
    }

    private async void OnUpdateClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_selectedPet == null)
            {
                await DisplayAlertAsync("Warning", "Please select a pet to update.", "OK");
                return;
            }

            // Validation
            if (PickerOwner.SelectedIndex < 0 ||
                string.IsNullOrWhiteSpace(EntryPetName.Text) ||
                PickerSpecies.SelectedIndex < 0 ||
                PickerGender.SelectedIndex < 0)
            {
                await DisplayAlertAsync("Validation Error", "Please fill in all required fields.", "OK");
                return;
            }

            if (!int.TryParse(EntryAge.Text, out int age) || age < 0)
            {
                await DisplayAlertAsync("Validation Error", "Please enter a valid age.", "OK");
                return;
            }

            var selectedOwner = _clientItems[PickerOwner.SelectedIndex];

            _selectedPet.Name = EntryPetName.Text.Trim();
            _selectedPet.Species = PickerSpecies.SelectedItem?.ToString() ?? string.Empty;
            _selectedPet.Age = age;
            _selectedPet.Gender = PickerGender.SelectedItem?.ToString() ?? string.Empty;
            _selectedPet.ClinicalNotes = EditorNotes.Text?.Trim() ?? string.Empty;
            _selectedPet.ClientId = selectedOwner.Id;

            _service.UpdatePet(_selectedPet);
            LblStatus.Text = "✅ Pet updated successfully!";
            LblStatus.TextColor = Color.FromArgb("#F4A261");

            ClearForm();
            LoadPets();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to update pet: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_selectedPet == null)
            {
                await DisplayAlertAsync("Warning", "Please select a pet to delete.", "OK");
                return;
            }

            bool confirm = await DisplayAlertAsync("Confirm Delete",
                $"Are you sure you want to delete {_selectedPet.Name}?\n\nThis will also delete all associated appointments.",
                "Yes, Delete", "Cancel");

            if (!confirm) return;

            _service.DeletePet(_selectedPet.Id);
            LblStatus.Text = "🗑️ Pet deleted successfully!";
            LblStatus.TextColor = Color.FromArgb("#E76F51");

            ClearForm();
            LoadPets();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to delete pet: {ex.Message}", "OK");
        }
    }

    // ==========================================
    // SELECTION & FILTER
    // ==========================================

    private void OnPetSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Pet selected)
        {
            _selectedPet = selected;

            // Fill form fields with selected pet data
            EntryPetName.Text = selected.Name;
            EntryAge.Text = selected.Age.ToString();
            EditorNotes.Text = selected.ClinicalNotes;

            // Set species picker
            var speciesList = (IList<string>)PickerSpecies.ItemsSource;
            PickerSpecies.SelectedIndex = speciesList.IndexOf(selected.Species);

            // Set gender picker
            var genderList = (IList<string>)PickerGender.ItemsSource;
            PickerGender.SelectedIndex = genderList.IndexOf(selected.Gender);

            // Set owner picker
            var ownerIndex = _clientItems.FindIndex(c => c.Id == selected.ClientId);
            if (ownerIndex >= 0)
                PickerOwner.SelectedIndex = ownerIndex;

            // Enable edit/delete buttons
            BtnUpdate.IsEnabled = true;
            BtnDelete.IsEnabled = true;
            BtnSave.IsEnabled = false;

            LblStatus.Text = $"Selected: {selected.Name}";
            LblStatus.TextColor = Color.FromArgb("#457B9D");
        }
    }

    private void OnFilterOwnerChanged(object? sender, EventArgs e)
    {
        try
        {
            if (PickerFilterOwner.SelectedIndex <= 0)
            {
                // "All Owners" selected or nothing selected
                LoadPets();
                return;
            }

            // Get selected owner from filter (offset by 1 because of "All Owners" entry)
            var filterItems = (List<ClientDisplayItem>)PickerFilterOwner.ItemsSource;
            var selectedFilter = filterItems[PickerFilterOwner.SelectedIndex];

            // Use LINQ query from ClinicService
            var filteredPets = _service.GetPetsByClientId(selectedFilter.Id);
            PetListView.ItemsSource = filteredPets;
            LblRecordCount.Text = $"{filteredPets.Count} records";
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("Error", $"Filter failed: {ex.Message}", "OK");
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
        _selectedPet = null;

        EntryPetName.Text = string.Empty;
        EntryAge.Text = string.Empty;
        EditorNotes.Text = string.Empty;
        PickerOwner.SelectedIndex = -1;
        PickerSpecies.SelectedIndex = -1;
        PickerGender.SelectedIndex = -1;

        PetListView.SelectedItem = null;

        BtnSave.IsEnabled = true;
        BtnUpdate.IsEnabled = false;
        BtnDelete.IsEnabled = false;

        LblStatus.Text = string.Empty;
    }
}
