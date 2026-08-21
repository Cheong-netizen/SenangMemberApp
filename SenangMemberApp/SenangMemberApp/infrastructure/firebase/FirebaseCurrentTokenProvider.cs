#if ANDROID
using Android.Gms.Tasks;
using Android.Util;
using Firebase.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SenangMemberApp.Shared.Infrastructure.Firebase;

namespace SenangMemberApp.Infrastructure.Firebase;

internal static class FirebaseCurrentTokenProvider
{
    private const string LogTag = "SenangFCM";

    internal static void Refresh()
    {
        try
        {
#pragma warning disable CS0618 // Kept for compatibility with the currently bound Firebase SDK.
            FirebaseMessaging.Instance.GetToken()
                .AddOnCompleteListener(new TokenCompleteListener());
#pragma warning restore CS0618
        }
        catch (Exception ex)
        {
            Log.Warn(LogTag, $"Unable to request the current FCM token: {ex.GetType().Name}");
        }
    }

    internal static async System.Threading.Tasks.Task PersistAndRegisterAsync(string token)
    {
        try
        {
            await FirebaseTokenStore.SaveAsync(token);
            Log.Info(LogTag, "FCM registration token refreshed.");

            var registrationService = IPlatformApplication.Current?.Services
                .GetService<IPushDeviceRegistrationService>();

            if (registrationService is not null)
            {
                await registrationService.RegisterCurrentDeviceAsync(force: true);
            }
        }
        catch (Exception ex)
        {
            Log.Warn(LogTag, $"FCM token persistence/registration failed: {ex.GetType().Name}");
        }
    }

    private sealed class TokenCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (!task.IsSuccessful)
            {
                Log.Warn(LogTag, "Firebase did not return a current registration token.");
                return;
            }

            var token = task.Result?.ToString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                _ = PersistAndRegisterAsync(token);
            }
        }
    }
}
#endif
