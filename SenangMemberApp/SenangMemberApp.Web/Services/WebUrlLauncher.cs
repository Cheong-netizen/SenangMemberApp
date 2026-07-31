using System.Threading.Tasks;
using Microsoft.JSInterop;
using SenangMemberApp.Shared.Services.IService;

namespace SenangMemberApp.Web.Services
{
    public class WebUrlLauncher : IUrlLauncher
    {
        private readonly IJSRuntime _jsRuntime;

        public WebUrlLauncher(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> OpenUrlAsync(string url)
        {
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
