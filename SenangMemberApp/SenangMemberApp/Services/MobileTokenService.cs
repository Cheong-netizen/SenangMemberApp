using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Services
{
    public class MobileTokenService : ITokenService
    {
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string CompanyTokenKey = "company_token";
        private const string CompanyRefreshTokenKey = "company_refresh_token";
        public Task ClearAsync()
        {
            SecureStorage.Remove(AccessTokenKey);
            SecureStorage.Remove(RefreshTokenKey);
            return Task.CompletedTask;
        }

        public Task ClearCompanyAsync()
        {
            SecureStorage.Remove(CompanyTokenKey);
            SecureStorage.Remove(CompanyRefreshTokenKey);
            return Task.CompletedTask;
        }

        public async Task<string?> GetCompanyTokenAsync()
        {
            return await SecureStorage.GetAsync(CompanyTokenKey);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(AccessTokenKey);
        }

        public async Task SaveCompanyTokenAsync(string companyToken, string refreshToken)
        {
            await SecureStorage.SetAsync(CompanyTokenKey, companyToken);
            await SecureStorage.SetAsync(CompanyRefreshTokenKey, refreshToken);
        }

        public async Task SaveTokenAsync(string token, string refreshToken)
        {
            await SecureStorage.SetAsync(AccessTokenKey, token);
            await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
        }
    }
}
