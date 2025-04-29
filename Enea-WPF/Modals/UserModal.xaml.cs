using Generated.Client;
using System.Windows;

namespace Enea_WPF;

public partial class UserModal : Window
{
    private readonly MyApiClient _apiClient;

    public UserModal(MyApiClient apiClient, UserReadDto user = null)
    {
        InitializeComponent();
        _apiClient = apiClient;
        User = user ?? new UserReadDto();
        IsEditMode = user != null;
        DataContext = this;
    }
    public UserReadDto User { get; set; }
    public bool IsEditMode { get; set; }
    public List<string> Roles { get; } = new List<string> { "Admin", "User" };

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IsEditMode)
            {
                await _apiClient.UserPUTAsync(User.Id,
                    new UserUpdateDto { Name = User.Name, Email = User.Email, Role = User.Role });
            }
            else
            {
                await _apiClient.UserPOSTAsync(
                    new UserCreateDto { Name = User.Name, Email = User.Email, Role = User.Role });
            }
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}