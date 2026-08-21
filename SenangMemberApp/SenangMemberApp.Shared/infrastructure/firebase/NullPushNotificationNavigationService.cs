namespace SenangMemberApp.Shared.Infrastructure.Firebase;

public sealed class NullPushNotificationNavigationService : IPushNotificationNavigationService
{
    public event Action? NavigationRequested
    {
        add { }
        remove { }
    }

    public bool TryDequeueRoute(out string route)
    {
        route = string.Empty;
        return false;
    }
}
