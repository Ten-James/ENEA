using Generated.Client;
using System.Windows;

namespace Enea_WPF;

public partial class LoginPage : Window
{
    private readonly MyApiClient _apiClient;
    private readonly MainWindow _mainWindow;
    public LoginPage(MyApiClient apiClient, MainWindow mainWindow)
    {
        InitializeComponent();
        _apiClient = apiClient;
        _mainWindow = mainWindow;
    }

    private async void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            btnLogin.IsEnabled = false;
            txtErrorMessage.Text = "";

            var username = txtUsername.Text;
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtErrorMessage.Text = "Username and password are required.";
                return;
            }

            var loginRequest = new LoginRequest { Email = username, Password = password };

            var result = await _apiClient.LoginAsync(loginRequest);

            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                _apiClient.SetToken(result.Token);
                _mainWindow.Show();
                Close();
            }
            else
            {
                txtErrorMessage.Text = "Login failed. Please check your credentials.";
            }
        }
        catch (Exception ex)
        {
            txtErrorMessage.Text = $"Error during login: {ex.Message}";
        }
        finally
        {
            btnLogin.IsEnabled = true;
        }
    }
}