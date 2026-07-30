using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Pages
{
    public partial class EditProfile
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IUserProfileService UserProfileService { get; set; } = default!;
        private UserProfileResponseDTO UserProfile = new();
        private DateTime Birthday;

        protected override async Task OnInitializedAsync()
        {
            var response = await UserProfileService.GetUserProfile();
            if (response != null)
            {
                UserProfile = response.result;
            }

            if (UserProfile.BirthdayYear > 0 && UserProfile.BirthdayMonth > 0 && UserProfile.BirthdayDay > 0)
            {
                Birthday = new DateTime(
                    UserProfile.BirthdayYear,
                    UserProfile.BirthdayMonth,
                    UserProfile.BirthdayDay
                );
            }
        }

        private async Task SaveProfile()
        {
            var request = new UserProfileRequestDTO
            {
                Phone = UserProfile.Phone,
                AccountName = UserProfile.AccountName,
                MemberPassword = UserProfile.MemberPassword,
                BirthdayYear = Birthday.Year,
                BirthdayMonth = Birthday.Month,
                BirthdayDay = Birthday.Day,
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
