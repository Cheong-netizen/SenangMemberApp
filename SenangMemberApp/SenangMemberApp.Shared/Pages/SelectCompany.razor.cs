using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class SelectCompany : IDisposable
    {
        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IShopState ShopState { get; set; } = default!;

        [Inject]
        private IUserProfileService UserProfileService { get; set; } = default!;

        [Inject]
        private ITokenService tokenService { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        private string companySearchText = string.Empty;
        private bool isLoading = true;
        private bool isSettingsModalVisible = false;
        private bool isLogoutModalVisible = false;
        private UserProfileResponseDTO userProfileData = new();

        private string CurrentCulture => System.Globalization.CultureInfo.CurrentCulture.Name;

        private IEnumerable<CompanyResponseDTO> filteredCompanies
        {
            get
            {
                var list = ShopState.CompanyList;
                if (list == null) return Enumerable.Empty<CompanyResponseDTO>();

                if (string.IsNullOrWhiteSpace(companySearchText))
                {
                    return list;
                }
                return list.Where(c => c != null && c.ShopName != null && c.ShopName.Contains(companySearchText, StringComparison.OrdinalIgnoreCase));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            ShopState.OnStateChange += HandleStateChanged;
            await ShopState.InitializeAsync();

            try
            {
                var profileRes = await UserProfileService.GetUserProfile();
                if (profileRes?.result != null)
                {
                    userProfileData = profileRes.result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SelectCompany] UserProfile error: {ex.Message}");
            }

            isLoading = false;
        }

        private void HandleStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            ShopState.OnStateChange -= HandleStateChanged;
        }

        private async Task selectCompany(string companyCode, string shopName)
        {
            try
            {
                await ShopState.SetShop(companyCode, shopName);
                navManager.NavigateTo("/home", replace: true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SelectCompany] Error selecting company '{shopName}' ({companyCode}): {ex}");
            }
        }

        private void OpenSettingsModal()
        {
            isSettingsModalVisible = true;
        }

        private void CloseSettingsModal()
        {
            isSettingsModalVisible = false;
        }

        private void NavToProfile()
        {
            isSettingsModalVisible = false;
            navManager.NavigateTo("/profile");
        }

        private void NavToEditProfile()
        {
            isSettingsModalVisible = false;
            navManager.NavigateTo("/EditProfile");
        }

        private void NavToChangePassword()
        {
            isSettingsModalVisible = false;
            navManager.NavigateTo("/ChangePassword");
        }

        private async Task ChangeLanguage(string culture)
        {
            if (CurrentCulture != culture)
            {
                await JS.InvokeVoidAsync("localStorage.setItem", "selectedCulture", culture);
                navManager.NavigateTo(navManager.Uri, forceLoad: true);
            }
        }

        private void ShowLogoutConfirmation()
        {
            isLogoutModalVisible = true;
        }

        private void CancelLogout()
        {
            isLogoutModalVisible = false;
        }

        private async Task ConfirmLogout()
        {
            isLogoutModalVisible = false;
            isSettingsModalVisible = false;
            await ShopState.ResetStateAsync();
            await tokenService.ClearAsync();
            navManager.NavigateTo("/", replace: true);
        }
    }
}
