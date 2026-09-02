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
        private bool isDeleteConfirmModalVisible = false;
        private bool isPhonePadOpen = false;

        private void OpenPhonePad()
        {
            isPhonePadOpen = true;
        }

        private void OnPhoneSelected(string selectedPhone)
        {
            phone = selectedPhone;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            var response = await UserProfileService.GetUserProfile();
            if (response != null && response.result != null)
            {
                UserProfile = response.result;
            }
        }

        private void OnDeleteClicked()
        {
            // Reset error message on every new attempt
            errorMessage = "";

            // Normalize and compare phone numbers
            var inputPhone = phone?.Trim().Replace("-", "").Replace(" ", "").Replace("+", "");
            var userPhone = UserProfile.Phone?.Trim().Replace("-", "").Replace(" ", "").Replace("+", "");

            bool isPhoneValid = !string.IsNullOrEmpty(inputPhone) && string.Equals(inputPhone, userPhone, StringComparison.OrdinalIgnoreCase);
            bool isPasswordValid = !string.IsNullOrEmpty(password) && password == UserProfile.MemberPassword;

            if (isPhoneValid && isPasswordValid)
            {
                // Credentials match, prompt user with confirmation modal
                isDeleteConfirmModalVisible = true;
            }
            else
            {
                // Credentials do not match
                errorMessage = Loc["ErrorMessage"];
            }
        }

        private void CancelDelete()
        {
            isDeleteConfirmModalVisible = false;
        }

        private async Task ConfirmDelete()
        {
            isDeleteConfirmModalVisible = false;

            // Trigger deletion
            var request = new UserProfileRequestDTO
            {
                Phone = UserProfile.Phone,
                AccountName = UserProfile.AccountName,
                MemberPassword = UserProfile.MemberPassword,
                Gender = UserProfile.Gender,
                Email = UserProfile.Email
            };

            await UserProfileService.ChangeUserProfile(request);

            // Navigate to homepage with accountDeleted flag
            NavManager.NavigateTo("/home?accountDeleted=true");
        }

        private void GoBack()
        {
            NavManager.NavigateTo("/profile");
        }
    }
}