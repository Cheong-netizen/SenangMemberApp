using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface ITokenService
    {
        Task SaveTokenAsync(string token, string refreshToken);
        Task<string?> GetTokenAsync();
        Task ClearAsync();
        Task SaveCompanyTokenAsync(string companyToken, string refreshToken);
        Task<string?> GetCompanyTokenAsync();
        Task ClearCompanyAsync();
    }
}
