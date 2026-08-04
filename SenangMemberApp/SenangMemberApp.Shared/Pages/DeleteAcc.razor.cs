using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class DeleteAcc
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IUserProfileService UserProfileService { get; set; } = default!;

        private UserProfileResponseDTO UserProfile = new();

        private string phone = "";
        private string password = "";
        private string errorMessage = ""; // Used to show validation errors to the user

        protected override async Task OnInitializedAsync()
        {
            var response = await UserProfileService.GetUserProfile();
            if (response != null && response.result != null)
            {
                UserProfile = response.result;
            }
        }

        private async Task ConfirmDelete()
        {
            // Reset error message on every new attempt
            errorMessage = "";

            // Normalize and compare phone numbers
            var inputPhone = phone?.Trim().Replace("-", "").Replace(" ", "").Replace("+", "");
            var userPhone = UserProfile.Phone?.Trim().Replace("-", "").Replace(" ", "").Replace("+", "");

            bool isPhoneValid = !string.IsNullOrEmpty(inputPhone) && string.Equals(inputPhone, userPhone, StringComparison.OrdinalIgnoreCase);
            bool isPasswordValid = password == UserProfile.MemberPassword;

            if (isPhoneValid && isPasswordValid)
            {
                // 2. Credentials match, proceed to trigger deletion
                var request = new UserProfileRequestDTO
                {
                    Phone = UserProfile.Phone,
                    AccountName = UserProfile.AccountName,
                    MemberPassword = UserProfile.MemberPassword,
                    Gender = UserProfile.Gender,
                    Email = UserProfile.Email
                };

                await UserProfileService.ChangeUserProfile(request);

                // Navigate away after successful deletion request
                NavManager.NavigateTo("/");
            }
            else
            {
                // 3. Credentials do not match
                errorMessage = Loc["ErrorMessage"];
            }
        }

        private void GoBack()
        {
            NavManager.NavigateTo("/profile");
        }
    }
}