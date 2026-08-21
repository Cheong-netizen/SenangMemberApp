using SenangMemberApp.Shared.Infrastructure.Firebase;

namespace SenangMemberApp.Infrastructure.Firebase;

public sealed class PushNotificationNavigationService : IPushNotificationNavigationService
{
    private readonly object _sync = new();
    private string? _pendingRoute;

    public event Action? NavigationRequested;

    public void Enqueue(string? notificationType, string? bookingId)
    {
        var route = BuildRoute(notificationType, bookingId);

        lock (_sync)
        {
            _pendingRoute = route;
        }

        NavigationRequested?.Invoke();
    }

    public bool TryDequeueRoute(out string route)
    {
        lock (_sync)
        {
            if (string.IsNullOrWhiteSpace(_pendingRoute))
            {
                route = string.Empty;
                return false;
            }

            route = _pendingRoute;
            _pendingRoute = null;
            return true;
        }
    }

    private static string BuildRoute(string? notificationType, string? bookingId)
    {
        var isBookingNotification = notificationType is
            "booking_confirmed" or
            "booking_rescheduled" or
            "booking_cancelled" or
            "booking_reminder" or
            "booking_completed";

        if (isBookingNotification && !string.IsNullOrWhiteSpace(bookingId))
        {
            return $"/AppointmentDetails/{Uri.EscapeDataString(bookingId)}/Appointment";
        }

        return notificationType == "announcement" ? "/announcement" : "/home";
    }
}
