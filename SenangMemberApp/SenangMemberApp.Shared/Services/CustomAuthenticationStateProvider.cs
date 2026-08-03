using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SenangMemberApp.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenService _tokenService;

        public CustomAuthenticationStateProvider(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();

                if (string.IsNullOrWhiteSpace(token))
                {
                    // No token found, user is anonymous
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Token found, parse claims and authenticate
                var claims = JwtParser.ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwtAuthType");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                // Catches errors during prerendering when JS interop is unavailable
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // Call this method from your Login/Logout components to trigger a UI refresh
        public void NotifyUserStatusChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
