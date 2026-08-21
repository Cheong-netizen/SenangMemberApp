using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using SenangMemberApp.Infrastructure.Firebase;
using SenangMemberApp.Shared.Infrastructure.Firebase;

namespace SenangMemberApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FirebaseNotificationManager.CreateNotificationChannel(this);
            FirebaseNotificationManager.RequestNotificationPermission(this);
            FirebaseCurrentTokenProvider.Refresh();
            HandleNotificationIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            if (intent is not null)
            {
                Intent = intent;
            }
            HandleNotificationIntent(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();

            var registrationService = IPlatformApplication.Current?.Services
                .GetService<IPushDeviceRegistrationService>();

            if (registrationService is not null)
            {
                _ = registrationService.RegisterCurrentDeviceAsync();
            }
        }

        private static void HandleNotificationIntent(Intent? intent)
        {
            var notificationType = intent?.GetStringExtra("type");
            if (string.IsNullOrWhiteSpace(notificationType))
            {
                return;
            }

            var navigationService = IPlatformApplication.Current?.Services
                .GetService<IPushNotificationNavigationService>() as PushNotificationNavigationService;

            navigationService?.Enqueue(notificationType, intent?.GetStringExtra("bookingId"));
        }
    }
}
