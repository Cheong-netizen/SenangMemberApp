using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SenangMemberApp.Web.Services
{
    public class WebShopStateLocalManagement : IShopStateLocalManagement
    {
        private readonly ProtectedLocalStorage _localStorage;
        public WebShopStateLocalManagement(ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<(string id, string name)> GetShopSelection()
        {
            try
            {
                var idResult = await _localStorage.GetAsync<string>("SelectedShopId");
                var nameResult = await _localStorage.GetAsync<string>("SelectedShopName");

                // If we successfully found the data in the browser
                if (idResult.Success && nameResult.Success)
                {
                    // Return both values inside parentheses
                    return (idResult.Value, nameResult.Value);
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                // Delete invalid keys if cryptography payload is invalid (e.g. data protection keys changed)
                await _localStorage.DeleteAsync("SelectedShopId");
                await _localStorage.DeleteAsync("SelectedShopName");
            }

            // Fallback: Return your default ID and your default Shop Name
            return ("0", "All Shops");
        }

        public async Task SaveShopSelection(string id, string name)
        {
            await _localStorage.SetAsync("SelectedShopId", id);
            await _localStorage.SetAsync("SelectedShopName", name);
        }
    }
}
