namespace SenangMemberApp.Shared.Infrastructure.Firebase;

public interface IPushNotificationNavigationService
{
    event Action? NavigationRequested;

    bool TryDequeueRoute(out string route);
}
