using Generated.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;

namespace Enea_WPF;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
        var loginWindow = ServiceProvider.GetRequiredService<LoginPage>();
        loginWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(sp => new MyApiClient("http://localhost:5012", new HttpClient()));
        services.AddTransient<LoginPage>();

    }
}