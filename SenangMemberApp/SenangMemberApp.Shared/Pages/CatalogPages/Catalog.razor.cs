using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages.CatalogPages
{
    public partial class Catalog
    {
        // --- Injections ---
        [Inject]
        IServiceProducts ServiceProducts { get; set; } = default!;
        [Inject]
        NavigationManager navigationManager { get; set; } = default!;
        [Inject]
        IShopState ShopState { get; set; } = default!;
        [Inject]
        CompanyAC companyAC { get; set; } = default!;

        // --- State Variables ---
        private List<CompanyResponseDTO> shops = new();
        private bool shopListModalIsOpen = false;
        private string shopSearchText = string.Empty;

        private string currentShopId = string.Empty;
        private string currentShopName = string.Empty;

        // REMOVED: servicesSearchText and filteredServices 

        private List<ServicesModel> services = new();
        private bool servicesModalIsOpen = false;
        private ServicesModel serviceToShowInModal = new();
        private List<CatalogResponseDTO> companyCategories = new();
        private bool loading = false;

        // --- Computed Properties ---
        private IEnumerable<CompanyResponseDTO> filteredShops
        {
            get
            {
                if (string.IsNullOrWhiteSpace(shopSearchText))
                {
                    return shops;
                }
                else
                {
                    return shops.Where(s => s.ShopName.Contains(shopSearchText, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        // --- Lifecycle Methods ---
        protected override async Task OnInitializedAsync()
        {
            loading = true;
            await ShopState.InitializeAsync();

            currentShopId = ShopState.CurrentShopId;
            currentShopName = string.IsNullOrEmpty(ShopState.CurrentShopName) ? "Select a shop" : ShopState.CurrentShopName;

            shops = ShopState.CompanyList ?? new List<CompanyResponseDTO>();
            services = ServiceProducts.GetServices();

            var response = await companyAC.FetchCompanyCategory();
            if (response != null)
            {
                companyCategories = response.result ?? new List<CatalogResponseDTO>();
            }
            loading = false;
        }

        // --- Core Methods ---
        private async Task selectShop(string id, string name)
        {
            loading = true;
            await ShopState.SetShop(id, name);
            currentShopId = id;
            currentShopName = name;
            shopListModalIsOpen = false;

            // Reload categories based on the newly selected shop (if your backend supports it)
            var response = await companyAC.FetchCompanyCategory();
            if (response != null)
            {
                companyCategories = response.result ?? new List<CatalogResponseDTO>();
            }
            Debug.WriteLine(response);
            loading = false;
            StateHasChanged();
        }

        private void selectAllShop()
        {
            ShopState.SetShop("0", "Select a shop");
            currentShopId = "0";
            currentShopName = "Select a shop";
            shopListModalIsOpen = false;
            StateHasChanged();
        }

        private void toggleShopListModal()
        {
            shopListModalIsOpen = !shopListModalIsOpen;
        }

        private void navBack()
        {
            navigationManager.NavigateTo("/home");
        }

        private void closeServiceModal()
        {
            servicesModalIsOpen = false;
        }

        private async Task itemClicked(int serviceId)
        {
            servicesModalIsOpen = true;
            serviceToShowInModal = services.First(i => i.Id == serviceId);
        }

        private void navCategory(string categoryId)
        {
            navigationManager.NavigateTo($"/product/{categoryId}");
        }
    }
}