#if ANDROID
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace SenangMemberApp.Infrastructure.Firebase;

internal static class FirebaseNotificationManager
{
    internal const string ChannelId = "appointments";
    private const int NotificationPermissionRequestCode = 4201;
    private const string NotificationPermissionAskedKey = "firebase_notification_permission_asked";

    internal static void CreateNotificationChannel(Context context)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        var channel = new NotificationChannel(
            ChannelId,
            "Appointment notifications",
            NotificationImportance.High)
        {
            Description = "Booking reminders, updates, and appointment notifications"
        };
        var audioAttributes = new Android.Media.AudioAttributes.Builder()
            .SetUsage(Android.Media.AudioUsageKind.Notification)
            .SetContentType(Android.Media.AudioContentType.Sonification)
            .Build();
        channel.EnableVibration(true);
        channel.SetSound(Settings.System.DefaultNotificationUri, audioAttributes);

        var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
        manager?.CreateNotificationChannel(channel);
    }

    internal static void RequestNotificationPermission(Activity activity)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33) ||
            ContextCompat.CheckSelfPermission(activity, Manifest.Permission.PostNotifications) == Permission.Granted ||
            Preferences.Default.Get(NotificationPermissionAskedKey, false))
        {
            return;
        }

        Preferences.Default.Set(NotificationPermissionAskedKey, true);
        ActivityCompat.RequestPermissions(
            activity,
            new[] { Manifest.Permission.PostNotifications },
            NotificationPermissionRequestCode);
    }

    internal static void Show(Context context, string title, string body, IDictionary<string, string> data)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(33) &&
            ContextCompat.CheckSelfPermission(context, Manifest.Permission.PostNotifications) != Permission.Granted)
        {
            return;
        }

        var launchIntent = new Intent(context, typeof(MainActivity));
        launchIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        foreach (var item in data)
        {
            launchIntent.PutExtra(item.Key, item.Value);
        }

        var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            pendingIntentFlags |= PendingIntentFlags.Immutable;
        }

        var requestCode = Random.Shared.Next(1, int.MaxValue);
        var pendingIntent = PendingIntent.GetActivity(context, requestCode, launchIntent, pendingIntentFlags);
        if (pendingIntent is null)
        {
            return;
        }

        var notification = new NotificationCompat.Builder(context, ChannelId)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetStyle(new NotificationCompat.BigTextStyle().BigText(body))
            .SetAutoCancel(true)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate))
            .SetCategory(NotificationCompat.CategoryReminder)
            .SetWhen(DateTimeOffset.Now.ToUnixTimeMilliseconds())
            .SetShowWhen(true)
            .SetContentIntent(pendingIntent)
            .Build();

        if (notification is not null)
        {
            NotificationManagerCompat.From(context)?.Notify(requestCode, notification);
        }
    }
}
#endif
