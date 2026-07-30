using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.LoginDTO;
using SenangMemberApp.Shared.Models.DTO.LoginRequestDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequestDTO loginRequest);
        Task<ApiResponseRoot<CompanyTokenResultDTO>> GetCompanyTokenAsync(string companyId);
    }
}
