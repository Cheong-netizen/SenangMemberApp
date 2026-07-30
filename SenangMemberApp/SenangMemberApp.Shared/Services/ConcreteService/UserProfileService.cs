using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ProfileAC _profileAC;
        public UserProfileService(ProfileAC profileAC)
        {
            _profileAC = profileAC;
        }
        public async Task<ApiResponseRoot<UserProfileResponseDTO>> GetUserProfile()
        {
            var response = await _profileAC.FetchUserProfile();

            return response;
        }

        public async Task ChangeUserProfile(UserProfileRequestDTO requestBody)
        {
            await _profileAC.UpdateUserProfile(requestBody);
        }
    }
}
