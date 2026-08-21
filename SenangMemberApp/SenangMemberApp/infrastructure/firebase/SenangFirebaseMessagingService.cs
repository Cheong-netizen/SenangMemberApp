#if ANDROID
using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Messaging;

namespace SenangMemberApp.Infrastructure.Firebase;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public sealed class SenangFirebaseMessagingService : FirebaseMessagingService
{
    private const string LogTag = "SenangFCM";

#pragma warning disable CS0672, CS0618 // Firebase still invokes this callback when its registration token rotates.
    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);
        _ = FirebaseCurrentTokenProvider.PersistAndRegisterAsync(token);
    }
#pragma warning restore CS0672, CS0618

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);

        var title = message.GetNotification()?.Title;
        var body = message.GetNotification()?.Body;

        if (string.IsNullOrWhiteSpace(title) && message.Data.TryGetValue("title", out var dataTitle))
        {
            title = dataTitle;
        }

        if (string.IsNullOrWhiteSpace(body) && message.Data.TryGetValue("body", out var dataBody))
        {
            body = dataBody;
        }

        FirebaseNotificationManager.Show(
            this,
            string.IsNullOrWhiteSpace(title) ? "Senang Member App" : title,
            body ?? string.Empty,
            message.Data);
    }

}
#endif
