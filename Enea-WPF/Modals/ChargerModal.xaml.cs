using Generated.Client;
using System.Windows;

namespace Enea_WPF.Modals;

public partial class ChargerModal : Window
{
    private readonly MyApiClient _apiClient;
    private readonly ChargerReadDto _charger;

    public ChargerModal(MyApiClient apiClient, ChargerReadDto charger = null)
    {
        InitializeComponent();
        _apiClient = apiClient;
        _charger = charger;

        LoadChargerGroups();
        LoadChargerStatuses();

        if (_charger != null)
        {
            // Populate fields for editing
            ChargerCodeTextBox.Text = _charger.ChargerCode;
            ChargerGroupComboBox.SelectedValue = _charger.ChargerGroupId;
            ChargerStatusComboBox.SelectedItem = _charger.CurrentStatus;
        }
    }

    private async void LoadChargerGroups()
    {
        try
        {
            var chargerGroups = await _apiClient.ChargerGroupGETAsync(null, null);
            ChargerGroupComboBox.ItemsSource = chargerGroups.Items;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load charger groups: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadChargerStatuses()
    {
        ChargerStatusComboBox.ItemsSource = Enum.GetValues(typeof(ChargerStatus)).Cast<ChargerStatus>();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var chargerCode = ChargerCodeTextBox.Text;
            var chargerGroupId = (Guid)ChargerGroupComboBox.SelectedValue;
            var chargerStatus = (ChargerStatus)ChargerStatusComboBox.SelectedItem;

            if (_charger == null)
            {
                // Create new charger
                var newCharger = new ChargerCreateDto { ChargerCode = chargerCode, ChargerGroupId = chargerGroupId, CurrentStatus = chargerStatus };
                await _apiClient.ChargerPOSTAsync(newCharger);
            }
            else
            {
                // Update existing charger
                var updatedCharger = new ChargerUpdateDto { ChargerCode = chargerCode, ChargerGroupId = chargerGroupId, CurrentStatus = chargerStatus };
                await _apiClient.ChargerPUTAsync(_charger.Id, updatedCharger);
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving charger: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}