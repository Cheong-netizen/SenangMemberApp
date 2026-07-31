using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.ConcreteService;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSelectShop
    {
        [Inject]
        private NavigationManager navManager { get; set; } = default!;
        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;
        [Inject]
        private IShopState shopState { get; set; } = default!;
        private List<CompanyResponseDTO> shops => shopState.CompanyList ?? new List<CompanyResponseDTO>();
        private string shopSearchText = string.Empty;
        private IEnumerable<CompanyResponseDTO> filteredShops
        {
            get
            {
                if (shops == null)
                    return Enumerable.Empty<CompanyResponseDTO>();

                if (string.IsNullOrWhiteSpace(shopSearchText))
                {
                    return shops;
                }
                else
                {
                    return shops.Where(s => s != null && s.ShopName != null && s.ShopName.Contains(shopSearchText, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
        private void selectShop(string shopId, string shopName)
        {
            shopState.SetShop(shopId, shopName);
            appointmentState.SetBookingShop(shopId, shopName);
        }
        protected override async Task OnInitializedAsync()
        {
            //shops = storeService.GetAllShops();
            shopState.OnStateChange += StateHasChanged;

            if (shopState.CompanyList == null || shopState.CompanyList.Count == 0)
            {
                await shopState.InitializeAsync();
            }
        }
        public void navSelectOutlet()
        {
            if(appointmentState.selectedBookingShopId != "" && appointmentState.selectedBookingShopId != null)
            {
                navManager.NavigateTo("/AppointmentSelectOutlet");
            }
        }
        private void navBack()
        {
            navManager.NavigateTo("/appointment");
        }

        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks when navigating away
            shopState.OnStateChange -= StateHasChanged;
        }
    }
}