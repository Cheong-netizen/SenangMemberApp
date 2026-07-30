using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SenangMemberApp.Web.Services
{
    public class WebStoreTokenService : ITokenService
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public WebStoreTokenService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }
        public async Task ClearAsync()
        {
            await _sessionStorage.DeleteAsync("authToken");
            await _sessionStorage.DeleteAsync("refreshToken");
        }

        public async Task ClearCompanyAsync()
        {
            await _sessionStorage.DeleteAsync("authCompanyToken");
            await _sessionStorage.DeleteAsync("refreshCompanyToken");
        }
        
        public async Task<string?> GetCompanyTokenAsync()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>("authCompanyToken");
                return result.Success ? result.Value : null;
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                await _sessionStorage.DeleteAsync("authCompanyToken");
                await _sessionStorage.DeleteAsync("refreshCompanyToken");
                return null;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>("authToken");
                return result.Success ? result.Value : null;
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                await _sessionStorage.DeleteAsync("authToken");
                await _sessionStorage.DeleteAsync("refreshToken");
                return null;
            }
        }

        public async Task SaveCompanyTokenAsync(string companyToken, string refreshToken)
        {
            Console.WriteLine($"Saving token: {companyToken}");
            Console.WriteLine($"Saving token: {refreshToken}");

            await _sessionStorage.SetAsync("authCompanyToken", companyToken);
            await _sessionStorage.SetAsync("refreshCompanyToken", refreshToken);
        }

        public async Task SaveTokenAsync(string token, string refreshToken)
        {
            Console.WriteLine($"Saving token: {token}");
            Console.WriteLine($"Saving token: {refreshToken}");

            await _sessionStorage.SetAsync("authToken", token);
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
        }
    }
}
