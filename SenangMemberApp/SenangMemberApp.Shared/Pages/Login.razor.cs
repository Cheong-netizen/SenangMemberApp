using SenangMemberApp.Services;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO.LoginDTO;
using SenangMemberApp.Shared.Models.DTO.LoginRequestDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Login
    {
        private LoginRequestDTO loginRequest = new();
        private LoginResult loginResult = new();
        private string errorMessage = "";
        private bool isLoading = false;

        [Inject]
        IAuthService authService { get; set; } = default!;

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        // Make sure NavigationManager is injected in the class so we can use it here
        [Inject]
        NavigationManager navigationManager { get; set; } = default!;

        // ADD THIS METHOD: It runs right after the page renders in the browser
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Ask the AuthStateProvider (which checks your TokenService) for the current state
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();

                // If the user's identity is already authenticated...
                if (authState.User.Identity?.IsAuthenticated == true)
                {
                    // Bounce them straight to the home page!
                    navigationManager.NavigateTo("/home", replace: true);
                }
            }
        }

        private async Task HandleLogin()
        {
            errorMessage = "";
            isLoading = true;

            try
            {
                loginResult = await authService.LoginAsync(loginRequest);

                if (loginResult != null && loginResult.Success == true)
                {
                    // Notify Blazor that the user's auth state has changed!
                    var customAuthStateProvider = (CustomAuthenticationStateProvider)AuthStateProvider;
                    customAuthStateProvider.NotifyUserStatusChanged();

                    navigationManager.NavigateTo("/home", replace: true);
                }
                else
                {
                    var msg = loginResult?.message;
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        errorMessage = Loc["DefaultLoginError"];
                    }
                    else
                    {
                        errorMessage = msg;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Login] Exception: {ex.Message}");
                errorMessage = Loc["NetworkError"];
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}