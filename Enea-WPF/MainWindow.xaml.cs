using Enea_WPF.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Enea_WPF;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        // Set the initial content
        MainContent.Content = _serviceProvider.GetRequiredService<UserListPage>();
        ((UserListPage)MainContent.Content).LoadUsers();
    }

    private void NavigationPanel_UsersClick(object sender, RoutedEventArgs e)
    {
        MainContent.Content = _serviceProvider.GetRequiredService<UserListPage>();
        ((UserListPage)MainContent.Content).LoadUsers();
    }

    private void NavigationPanel_ChargingGroupsClick(object sender, RoutedEventArgs e)
    {
        MainContent.Content = _serviceProvider.GetRequiredService<ChargingGroupListPage>();
        ((ChargingGroupListPage)MainContent.Content).LoadUsers();
    }
    private void NavigationPanel_OnStatsClick(object sender, RoutedEventArgs e)
    {
        MainContent.Content = _serviceProvider.GetRequiredService<StatsPage>();
        ((StatsPage)MainContent.Content).LoadStats();
    }
    private void NavigationPanel_OnEventsClick(object sender, RoutedEventArgs e)
    {
        MainContent.Content = _serviceProvider.GetRequiredService<EventsPage>();
        ((EventsPage)MainContent.Content).LoadFromApi();
    }
    private void NavigationPanel_OnChargersClick(object sender, RoutedEventArgs e)
    {
        MainContent.Content = _serviceProvider.GetRequiredService<ChargerListPage>();
        ((ChargerListPage)MainContent.Content).LoadChargers();
    }
}