using SenangMemberApp.Shared.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IUserProfileService
    {
        Task<ApiResponseRoot<UserProfileResponseDTO>> GetUserProfile();
        Task ChangeUserProfile(UserProfileRequestDTO requestBody);
    }
}
