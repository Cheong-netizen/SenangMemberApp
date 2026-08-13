using Microsoft.AspNetCore.Components;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class SelectCompany : IDisposable
    {
        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IShopState ShopState { get; set; } = default!;

        private string companySearchText = string.Empty;
        private bool isLoading = true;

        private IEnumerable<CompanyResponseDTO> filteredCompanies
        {
            get
            {
                var list = ShopState.CompanyList;
                if (list == null) return Enumerable.Empty<CompanyResponseDTO>();

                if (string.IsNullOrWhiteSpace(companySearchText))
                {
                    return list;
                }
                return list.Where(c => c != null && c.ShopName != null && c.ShopName.Contains(companySearchText, StringComparison.OrdinalIgnoreCase));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            ShopState.OnStateChange += HandleStateChanged;
            await ShopState.InitializeAsync();
            isLoading = false;
        }

        private void HandleStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            ShopState.OnStateChange -= HandleStateChanged;
        }

        private async Task selectCompany(string companyCode, string shopName)
        {
            await ShopState.SetShop(companyCode, shopName);
            navManager.NavigateTo("/home", replace: true);
        }
    }
}
