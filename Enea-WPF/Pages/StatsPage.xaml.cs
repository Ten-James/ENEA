using Generated.Client;
using System.Windows;
using System.Windows.Controls;

namespace Enea_WPF.Pages;

public partial class StatsPage : UserControl
{
    private readonly MyApiClient _apiClient;

    public StatsPage(MyApiClient apiClient)
    {
        _apiClient = apiClient;
        InitializeComponent();
    }

    public async void LoadStats()
    {
        try
        {
            var stats = await _apiClient.AdminAsync();
            dataGridStats.ItemsSource = stats;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}