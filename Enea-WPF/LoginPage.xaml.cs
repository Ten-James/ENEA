using Generated.Client;
using System.Windows;
using System.Windows.Controls;

namespace Enea_WPF;

public partial class LoginPage : Window
{
    private readonly MyApiClient _apiClient;
    public LoginPage(MyApiClient apiClient)
    {
        InitializeComponent();
        _apiClient = apiClient;
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
                // Store token in shared memory

                // Open user management page
                //var userPage = App.ServiceProvider.GetRequiredService<UserManagementPage>();
                //userPage.Show();

                // Close login window
                //this.Close();
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