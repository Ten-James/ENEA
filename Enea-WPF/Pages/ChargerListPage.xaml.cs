using Enea_WPF.Modals;
using Generated.Client;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Enea_WPF.Pages;

public partial class ChargerListPage : UserControl
{
    private readonly MyApiClient _apiClient;

    public ChargerListPage(MyApiClient apiClient)
    {
        _apiClient = apiClient;
        InitializeComponent();
        LoadChargers();
    }
    public ObservableCollection<ChargerReadDto> Chargers { get; set; } = new ObservableCollection<ChargerReadDto>();

    public async void LoadChargers()
    {
        try
        {
            var chargers = await _apiClient.ChargerGETAsync(0, 0);
            Chargers.Clear();
            foreach (var charger in chargers.Items)
            {
                Chargers.Add(charger);
            }
            dataGridChargers.ItemsSource = Chargers;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load chargers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CreateChargerButton_Click(object sender, RoutedEventArgs e)
    {
        var modal = new ChargerModal(_apiClient);
        if (modal.ShowDialog() == true)
        {
            LoadChargers();
        }
    }

    private void EditChargerButton_Click(object sender, RoutedEventArgs e)
    {
        if (dataGridChargers.SelectedItem is ChargerReadDto selectedCharger)
        {
            var modal = new ChargerModal(_apiClient, selectedCharger);
            if (modal.ShowDialog() == true)
            {
                LoadChargers();
            }
        }
        else
        {
            MessageBox.Show("Please select a charger to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void DeleteChargerButton_Click(object sender, RoutedEventArgs e)
    {
        if (dataGridChargers.SelectedItem is ChargerReadDto selectedCharger)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the charger '{selectedCharger.ChargerCode}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiClient.ChargerDELETEAsync(selectedCharger.Id);
                    Chargers.Remove(selectedCharger);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete charger: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Please select a charger to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}