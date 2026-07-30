using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class ShopState : IShopState
    {
        private readonly ICompanyService _companyService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly IShopStateLocalManagement _shopStateLocalManagement;
        public List<CompanyResponseDTO> CompanyList { get; private set; } = new();
        public string CurrentShopId { get; private set; } = "0";
        public string CurrentShopName { get; private set; } = "All Shops";
        public ApiResponseRoot<List<CreditResponseDTO>> currentCreditDetails { get; private set; }
        public double balanceCredit { get; private set; }
        public MemberBalanceDTO currentMemberBalance { get; private set; } = new();
        public decimal currentPackageBalance { get; private set; }
        public event Action OnStateChange;
        private void NotifyStateChanged() => OnStateChange?.Invoke();
        public bool IsLoading { get; private set; } = true;
        public ShopState(ICompanyService companyService, ITokenService tokenService, IAuthService authService, IShopStateLocalManagement shopStateLocalManagement)
        {
            _companyService = companyService;
            _tokenService = tokenService;
            _authService = authService;
            _shopStateLocalManagement = shopStateLocalManagement;
        }
        public async Task InitializeAsync()
        {
            IsLoading = true;
            NotifyStateChanged(); // Tell UI we are loading

            var response = await _companyService.GetCompanyList();

            if (response == null || response.result == null)
            {
                CompanyList = new List<CompanyResponseDTO>();
                IsLoading = false;
                NotifyStateChanged();
                return;
            }
            await RestoreState();

            CompanyList = response.result;

            IsLoading = false;
            NotifyStateChanged(); // Tell UI initialization is done
        }
        public async Task SetShop(string shopId, string shopName)
        {
            IsLoading = true;
            NotifyStateChanged();

            CurrentShopId = shopId;
            CurrentShopName = shopName;

            if (shopId == "0")
            {
                IsLoading = false;
                NotifyStateChanged();
                await _shopStateLocalManagement.SaveShopSelection(shopId, shopName);
                return;
            }

            await setCompanyToken(shopId);
            await setMemberBalanceDetails();
            await setPackageDetails();
            await _shopStateLocalManagement.SaveShopSelection(shopId, shopName);

            Debug.WriteLine(currentCreditDetails);

            IsLoading = false;
            NotifyStateChanged(); // Tell UI the new shop data is ready
        }

        public async Task setCompanyToken(string id)
        {
            ApiResponseRoot<CompanyTokenResultDTO> response = await _authService.GetCompanyTokenAsync(id);

            Debug.WriteLine($"GetCompanyTokenAsync response: {response.statusCode} - {response.message}");

            await _tokenService.SaveCompanyTokenAsync(response.result.AccessToken, response.result.RefreshToken);
        }
        public async Task setMemberBalanceDetails()
        {
            currentMemberBalance = await _companyService.GetCompanyMemberBalance();
        }

        public async Task setPackageDetails()
        {
            var packageResponse = await _companyService.GetCompanyPackageDetails();

            if (packageResponse != null && packageResponse.result != null)
            {
                currentPackageBalance = packageResponse.result.Sum(p => p.quantityAvailable);
            }
        }
        private async Task RestoreState()
        {
            var (savedId, savedName) = await _shopStateLocalManagement.GetShopSelection();

            if (savedId != "0")
            {
                await SetShop(savedId, savedName);
            }
        }
    }
}
