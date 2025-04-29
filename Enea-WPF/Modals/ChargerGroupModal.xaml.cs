using Generated.Client;
using System.Windows;

namespace Enea_WPF.Modals;

public partial class ChargerGroupModal : Window
{
    private readonly MyApiClient _apiClient;
    public ChargerGroupReadDto ChargerGroup;

    public ChargerGroupModal(MyApiClient apiClient, ChargerGroupReadDto chargerGroup = null)
    {
        InitializeComponent();
        _apiClient = apiClient;
        ChargerGroup = chargerGroup;

        if (ChargerGroup != null)
        {
            // Populate fields for editing
            NameTextBox.Text = ChargerGroup.Name;
            LatitudeTextBox.Text = ChargerGroup.Latitude.ToString();
            LongitudeTextBox.Text = ChargerGroup.Longitude.ToString();
            AddressTextBox.Text = ChargerGroup.Address;
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var name = NameTextBox.Text;
            var latitude = double.Parse(LatitudeTextBox.Text);
            var longitude = double.Parse(LongitudeTextBox.Text);
            var address = AddressTextBox.Text;

            if (ChargerGroup == null)
            {
                // Create new charger group
                var newGroup = new ChargerGroupCreateDto { Name = name, Latitude = latitude, Longitude = longitude, Address = address };
                await _apiClient.ChargerGroupPOSTAsync(newGroup);
            }
            else
            {
                // Update existing charger group
                var updatedGroup = new ChargerGroupUpdateDto { Name = name, Latitude = latitude, Longitude = longitude, Address = address };
                await _apiClient.ChargerGroupPUTAsync(ChargerGroup.Id, updatedGroup);
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving charger group: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}