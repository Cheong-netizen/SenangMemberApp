using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Profile
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IUserProfileService UserProfileService { get; set; } = default!;
        [Inject]
        private ITokenService tokenService { get; set; } = default!;
        private UserProfileResponseDTO UserProfile { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            var response = await UserProfileService.GetUserProfile();
            if (response != null)
            {
                UserProfile = response.result ?? new();
            }
            StateHasChanged();
        }
        private void NavToEditProfile()
        {
            // Navigate to your Edit Profile page
            NavManager.NavigateTo("/EditProfile");
        }
        private void NavToDeleteAcc()
        {
            NavManager.NavigateTo("/deleteAccount");
        }
        private async Task Logout()
        {
            // 1. Clear User State/Tokens here
            // 2. Navigate to Login Page
            await tokenService.ClearAsync();
            NavManager.NavigateTo("/");
        }

        private void NavToChangePassword()
        {
            NavManager.NavigateTo("/ChangePassword");
        }
        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        // Track the current culture for the UI buttons
        private string CurrentCulture => System.Globalization.CultureInfo.CurrentCulture.Name;

        // ... keep your existing OnInitializedAsync and Nav methods ...

        private async Task ChangeLanguage(string culture)
        {
            if (CurrentCulture != culture)
            {
                // 1. Save to Local Storage
                await JS.InvokeVoidAsync("localStorage.setItem", "selectedCulture", culture);

                // 2. Force a reload to apply the culture globally to the entire app
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
        }
        private bool IsLogoutModalVisible { get; set; } = false;

        private void ShowLogoutConfirmation()
        {
            IsLogoutModalVisible = true;
        }

        private void CancelLogout()
        {
            IsLogoutModalVisible = false;
        }

        private async Task ConfirmLogout()
        {
            IsLogoutModalVisible = false;
            await tokenService.ClearAsync();
            NavManager.NavigateTo("/");
        }
    }
}
