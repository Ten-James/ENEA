using System.Windows;
using System.Windows.Controls;

namespace Enea_WPF;

public partial class NavigationPanel : UserControl
{
    // Define Routed Events
    public static readonly RoutedEvent UsersClickEvent = EventManager.RegisterRoutedEvent(
        nameof(UsersClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NavigationPanel));

    public static readonly RoutedEvent ChargingGroupsClickEvent = EventManager.RegisterRoutedEvent(
        nameof(ChargingGroupsClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NavigationPanel));

    public static readonly RoutedEvent StatsClickEvent = EventManager.RegisterRoutedEvent(
        nameof(StatsClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NavigationPanel));

    public static readonly RoutedEvent EventsClickEvent = EventManager.RegisterRoutedEvent(
        nameof(EventsClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NavigationPanel));

    public static readonly RoutedEvent ChargersClickEvent = EventManager.RegisterRoutedEvent(
        nameof(ChargersClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NavigationPanel));

    public NavigationPanel()
    {
        InitializeComponent();
    }

    // CLR Event Wrappers
    public event RoutedEventHandler UsersClick
    {
        add => AddHandler(UsersClickEvent, value);
        remove => RemoveHandler(UsersClickEvent, value);
    }

    public event RoutedEventHandler ChargingGroupsClick
    {
        add => AddHandler(ChargingGroupsClickEvent, value);
        remove => RemoveHandler(ChargingGroupsClickEvent, value);
    }

    public event RoutedEventHandler StatsClick
    {
        add => AddHandler(StatsClickEvent, value);
        remove => RemoveHandler(StatsClickEvent, value);
    }

    public event RoutedEventHandler EventsClick
    {
        add => AddHandler(EventsClickEvent, value);
        remove => RemoveHandler(EventsClickEvent, value);
    }

    public event RoutedEventHandler ChargersClick
    {
        add => AddHandler(ChargersClickEvent, value);
        remove => RemoveHandler(ChargersClickEvent, value);
    }

    private void UsersButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(UsersClickEvent));
    }

    private void ChargingGroupButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ChargingGroupsClickEvent));
    }

    private void StatsButton_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(StatsClickEvent));
    }

    private void EventsButton_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(EventsClickEvent));
    }

    private void ChargersButton_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ChargersClickEvent));
    }
}