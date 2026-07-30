using Android.Webkit;
using Microsoft.Maui.ApplicationModel;

namespace SenangMemberApp;

/// <summary>
/// Grants microphone requests made by content hosted in the MAUI BlazorWebView.
/// Android treats this separately from the app-level RECORD_AUDIO permission.
/// </summary>
internal sealed class MicrophoneWebChromeClient : WebChromeClient
{
    public override void OnPermissionRequest(PermissionRequest? request)
    {
        if (request is null)
        {
            return;
        }

        var requestedResources = request.GetResources() ?? [];
        var requestsMicrophone = requestedResources.Any(resource =>
            string.Equals(
                resource,
                PermissionRequest.ResourceAudioCapture,
                StringComparison.OrdinalIgnoreCase));

        if (!requestsMicrophone)
        {
            request.Deny();
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Microphone>();
                }

                if (status == PermissionStatus.Granted)
                {
                    request.Grant([PermissionRequest.ResourceAudioCapture]);
                }
                else
                {
                    request.Deny();
                }
            }
            catch
            {
                request.Deny();
            }
        });
    }
}
