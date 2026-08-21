using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.LoginDTO;
using SenangMemberApp.Shared.Models.DTO.LoginRequestDTO;
using SenangMemberApp.Shared.Infrastructure.Firebase;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class AuthService : IAuthService
    {
        private readonly AuthAC _authAC;
        private readonly ITokenService _tokenService;
        private readonly IPushDeviceRegistrationService _pushDeviceRegistrationService;
        //private readonly IShopState _shopState;
        public AuthService(
            AuthAC authAC,
            ITokenService tokenService,
            IPushDeviceRegistrationService pushDeviceRegistrationService)
        {
            _authAC = authAC;
            _tokenService = tokenService;
            _pushDeviceRegistrationService = pushDeviceRegistrationService;
            //_shopState = shopState;
        }
        public async Task<LoginResult> LoginAsync(LoginRequestDTO loginRequest)
        {
            ApiResponseRoot<LoginResponseDTO> response = await _authAC.LoginAsync(loginRequest);

            if (response == null)
            {
                return new LoginResult
                {
                    Success = false,
                    message = "Fail to request from API"
                };
            }

            if (response.statusCode == 200 && response.result?.authResponse?.accessToken != null)
            {
                await _tokenService.ClearCompanyAsync();
                await _tokenService.SaveTokenAsync(response.result.authResponse.accessToken, response.result.authResponse.refreshToken);
                // Registration is best-effort and must not turn a valid login into a failure.
                _ = _pushDeviceRegistrationService.RegisterCurrentDeviceAsync(force: true);
                //await _shopState.InitializeAsync();
                return new LoginResult
                {
                    Success = true,
                    message = response.message
                };
            }

            string errorMsg = !string.IsNullOrWhiteSpace(response.message)
                ? response.message
                : "Email or password is wrong";

            return new LoginResult
            {
                Success = false,
                message = errorMsg
            };
        }

        public async Task<ApiResponseRoot<CompanyTokenResultDTO>> GetCompanyTokenAsync(string companyId)
        {
            ApiResponseRoot<CompanyTokenResultDTO> response = await _authAC.GetCompanyTokenAsync(companyId);

            return response;
        }
    }
}
