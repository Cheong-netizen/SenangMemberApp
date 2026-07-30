using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.LoginDTO;
using SenangMemberApp.Shared.Models.DTO.LoginRequestDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SenangMemberApp.Shared.ApiClient
{
    public class AuthAC : BaseAC
    {
        public AuthAC(HttpClient httpClient, ITokenService tokenService) : base(httpClient, tokenService)
        {
        }

        public async Task<ApiResponseRoot<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest)
        {
            var response = await PostAsync<LoginRequestDTO, ApiResponseRoot<LoginResponseDTO>>("api/PublicMember/login", loginRequest);

            if (response == null)
            {
                return new ApiResponseRoot<LoginResponseDTO>
                {
                    statusCode = 500,
                    message = "An error occurred while processing the request.",
                    result = null
                };
            }

            return response;
        }

        public async Task<ApiResponseRoot<CompanyTokenResultDTO>> GetCompanyTokenAsync(string companyId)
        {
            var payload = new { id = companyId };

            var response = await PostAsync<object, ApiResponseRoot<CompanyTokenResultDTO>>("api/PublicMember/LoginCompany", payload);
            if (response == null)
            {
                return new ApiResponseRoot<CompanyTokenResultDTO>
                {
                    statusCode = 500,
                    message = "An error occurred while processing the request.",
                    result = null
                };
            }

            Debug.WriteLine(response);

            return response;
        }

        public async Task<ApiResponseRoot<UserProfileResponseDTO>> GetUserProfile()
        {
            var response = await PostAsync<object, ApiResponseRoot<UserProfileResponseDTO>>("api/PublicMember/GetProfile", null);

            var objSetting = new System.Text.Json.JsonSerializerOptions{ WriteIndented = true};
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, objSetting);

            return response;
        }
    }
}
