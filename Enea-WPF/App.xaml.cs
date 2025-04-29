using Enea_WPF.Pages;
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

        ServiceProvider = ConfigureServices();
        var mainWindow = ServiceProvider.GetRequiredService<LoginPage>();
        mainWindow.Show();
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(sp => new MyApiClient("http://localhost:5012", new HttpClient()));
        services.AddSingleton<LoginPage>();
        services.AddSingleton<UserListPage>();
        services.AddSingleton<ChargingGroupListPage>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<StatsPage>();
        services.AddSingleton<EventsPage>();
        services.AddSingleton<ChargerListPage>();

        return services.BuildServiceProvider();
    }
}