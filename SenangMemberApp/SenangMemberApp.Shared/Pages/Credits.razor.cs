using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Credits : IDisposable
    {
        [Inject]
        IShopState ShopState { get; set; } = default!;
        [Inject]
        ICompanyService CompanyService { get; set; } = default!;
        private List<CreditResponseDTO> creditDetails = new();
        protected override async Task OnInitializedAsync()
        {
            ShopState.OnStateChange += StateHasChanged;
            
            // If ShopState hasn't started loading yet, trigger it here 
            // (or ensure it's triggered in your MainLayout)
            if (ShopState.IsLoading && (ShopState.CompanyList == null || ShopState.CompanyList.Count == 0))
            {
                await ShopState.InitializeAsync();
            }

            await getCompanyBalanceDetails();
        }
        private async Task getCompanyBalanceDetails()
        {
            var response = await CompanyService.GetCompanyCreditBalanceDetails();
            if(response != null)
            {
                creditDetails = response.result ?? new List<CreditResponseDTO>();
                StateHasChanged();
            }
        }
        private void GoBack()
        {
            NavManager.NavigateTo("/home");
        }


        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks when navigating away
            ShopState.OnStateChange -= StateHasChanged;
        }
    }
}
