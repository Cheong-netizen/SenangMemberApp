using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using SenangMemberApp.Shared.Services.IService;

namespace SenangMemberApp.Services
{
    public class MobileUrlLauncher : IUrlLauncher
    {
        private readonly IJSRuntime _jsRuntime;

        public MobileUrlLauncher(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> OpenUrlAsync(string url)
        {
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    bool launched = await Launcher.Default.OpenAsync(uri);
                    if (launched)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MobileUrlLauncher] Launcher.Default.OpenAsync failed for '{url}': {ex.Message}");
            }

            // Fallback to JS if Launcher fails
            try
            {
                await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
