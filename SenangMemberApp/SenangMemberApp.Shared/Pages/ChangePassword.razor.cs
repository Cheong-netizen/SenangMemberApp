using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class ChangePassword
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IUserProfileService UserProfileService { get; set; } = default!;

        private UserProfileResponseDTO UserProfile = new();

        // New properties for the password change form
        private string CurrentPassword { get; set; } = string.Empty;
        private string NewPassword { get; set; } = string.Empty;
        private string ConfirmPassword { get; set; } = string.Empty;
        private string ErrorMessage { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var response = await UserProfileService.GetUserProfile();
            if (response != null && response.result != null)
            {
                UserProfile = response.result;
            }
        }

        private async Task SaveProfile()
        {
            ErrorMessage = string.Empty; // Reset error state

            // 1. Check if fields are empty
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Please fill in all password fields.";
                return;
            }

            // 2. Validate that the new passwords match
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "The new passwords do not match.";
                return;
            }

            // 3. Optional client-side check for current password
            // Note: In production, it's safer to have the API validate the current password. 
            if (!string.IsNullOrEmpty(UserProfile.MemberPassword) && CurrentPassword != UserProfile.MemberPassword)
            {
                ErrorMessage = "The current password you entered is incorrect.";
                return;
            }

            // 4. Map the existing profile data, but swap in the NewPassword
            var request = new UserProfileRequestDTO
            {
                Phone = UserProfile.Phone,
                AccountName = UserProfile.AccountName,
                MemberPassword = NewPassword, // Pass the new password here
                BirthdayYear = UserProfile.BirthdayYear,
                BirthdayMonth = UserProfile.BirthdayMonth,
                BirthdayDay = UserProfile.BirthdayDay,
                Gender = UserProfile.Gender,
                Email = UserProfile.Email
            };

            await UserProfileService.ChangeUserProfile(request);
            NavManager.NavigateTo("/profile");
        }

        private void GoBack()
        {
            NavManager.NavigateTo("/profile");
        }
    }
}