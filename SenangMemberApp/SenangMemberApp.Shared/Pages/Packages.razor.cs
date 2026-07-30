using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Packages : IDisposable
    {
        [Inject]
        IShopState ShopState { get; set; } = default!;
        [Inject]
        ICompanyService CompanyService { get; set; } = default!;
        private List<PackageResponseDTO> packageDetails = new();
        protected override async Task OnInitializedAsync()
        {
            ShopState.OnStateChange += StateHasChanged;

            // If ShopState hasn't started loading yet, trigger it here 
            // (or ensure it's triggered in your MainLayout)
            if (ShopState.IsLoading && ShopState.CompanyList.Count == 0)
            {
                await ShopState.InitializeAsync();
            }
            await setPackageDetails();
        }
        
        private void GoBack()
        {
            NavManager.NavigateTo("/home");
        }
        private async Task setPackageDetails()
        {
            var response = await CompanyService.GetCompanyPackageDetails();
            if (response != null && response.result != null)
            {
                packageDetails = response.result;
            }
        }

        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks when navigating away
            ShopState.OnStateChange -= StateHasChanged;
        }
    }
}
