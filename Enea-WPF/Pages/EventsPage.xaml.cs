using Generated.Client;
using System.Windows;
using System.Windows.Controls;

namespace Enea_WPF.Pages;

public partial class EventsPage : UserControl
{
    private const int PageSize = 100;
    private readonly MyApiClient _apiClient;
    private int _currentPage = 1;
    private int _totalPages = 1;

    public EventsPage(MyApiClient apiClient)
    {
        _apiClient = apiClient;
        InitializeComponent();
    }

    public async void LoadFromApi()
    {
        try
        {
            var data = await _apiClient.ChargerEventsAsync(_currentPage, PageSize);
            dataGrid.ItemsSource = data.Items;

            // Update page information
            _totalPages = (int)Math.Ceiling((double)data.TotalCount / PageSize);
            pageLabel.Text = $"Page: {_currentPage}";
            maxPageLabel.Text = $"of {_totalPages}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PreviousButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            LoadFromApi();
        }
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            LoadFromApi();
        }
    }
}