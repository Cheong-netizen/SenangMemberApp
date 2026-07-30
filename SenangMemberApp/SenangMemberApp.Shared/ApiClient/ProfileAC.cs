using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SenangMemberApp.Shared.ApiClient
{
    public class ProfileAC : BaseAC
    {
        public ProfileAC(ITokenService tokenService, HttpClient httpClient) : base(httpClient, tokenService)
        {

        }

        public async Task<ApiResponseRoot<UserProfileResponseDTO>> FetchUserProfile()
        {
            var response = await PostAsync<object, ApiResponseRoot<UserProfileResponseDTO>>("/api/PublicMember/GetProfile", null);

            var objSetting = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var debugResponse = System.Text.Json.JsonSerializer.Serialize(response, objSetting);

            Debug.WriteLine(debugResponse);

            return response;
        }

        public async Task UpdateUserProfile(UserProfileRequestDTO requestBody)
        {
            var response = await PostAsync<UserProfileRequestDTO, object>("api/PublicMember/UpdateProfile", requestBody);

            var objSetting = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var debugResponse = System.Text.Json.JsonSerializer.Serialize(response, objSetting);

            Debug.WriteLine(debugResponse);
        }
    }
}
