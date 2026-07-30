using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.PurchaseHistoryDTO;
using SenangMemberApp.Shared.Services.ConcreteService;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Pages
{
    public partial class History
    {
        private List<PurchaseHistoryMonthlyResponseDTO> availableMonth;
        private string currentShopId;
        private string currentShopName = "";
        private bool shopListModalIsOpen = false;
        private List<ShopModel> shops = new();
        private string shopSearchText = string.Empty;
        ApiResponseRoot<Dictionary<string, List<ServiceRecordResponseDTO>>> AllHistory;
        private bool loading = false;

        [Inject]
        NavigationManager navigationManager { get; set; } = default!;
        [Inject]
        PurchaseHistoryAC purchaseHistoryAC { get; set; } = default!;
        [Inject]
        IShopState shopState { get; set; } = default!;
        private bool isReviewModalOpen = false;
        protected override async Task OnInitializedAsync()
        {
            loading = true;
            var response = await purchaseHistoryAC.GetCustomerServiceRecordByMonthAsync();
            availableMonth = response?.result ?? new List<PurchaseHistoryMonthlyResponseDTO>();
            currentShopName = shopState.CurrentShopName;
            currentShopId = shopState.CurrentShopId;
            loading = false;
        }
        private IEnumerable<CompanyResponseDTO> filteredShops
        {
            get
            {
                if (string.IsNullOrWhiteSpace(shopSearchText))
                {
                    return ShopState.CompanyList;
                }
                else
                {
                    return ShopState.CompanyList
                        .Where(s => s.ShopName.Contains(shopSearchText, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
        [Inject]
        IShopState ShopState { get; set; } = default!;
        private void OpenReviewModal()
        {
            isReviewModalOpen = true;
        }
        private void CloseReviewModal()
        {
            isReviewModalOpen = false;
        }
        private void toggleShopListModal()
        {
            shopListModalIsOpen = !shopListModalIsOpen;
        }
        private void selectAllShop()
        {
            ShopState.SetShop("0", "All Shops");
            // Close the modal
            shopListModalIsOpen = false;
        }
        // Updated selectShop to handle data reloading
        private async Task selectShop(string id, string name)
        {
            // 1. Close the modal
            shopListModalIsOpen = false;
            loading = true;

            // 2. Update the global state
            await ShopState.SetShop(id, name);

            // 3. Update local variables for UI sync
            currentShopId = id;
            currentShopName = name;
            AllHistory = new();
            availableMonth = new();

            // 4. Reload History based on the new shop selection
            await LoadHistoryData();
            loading = false;
        }
        //void OpenDetails(int historyId)
        //{
        //    navigationManager.NavigateTo($"/history/details/{historyId}");
        //}
        private string selectedMonth = "";
        private async Task LoadHistoryData()
        {
            var response = await purchaseHistoryAC.GetCustomerServiceRecordByMonthAsync();
            availableMonth = response?.result ?? new List<PurchaseHistoryMonthlyResponseDTO>();
            if (!string.IsNullOrEmpty(selectedMonth))
            {
                AllHistory = await purchaseHistoryAC.GetCustomerServiceRecordByMonthAsync(selectedMonth);
                StateHasChanged();
            }
        }
        // The method triggered by the dropdown
        private async void OnMonthSelected(ChangeEventArgs e)
        {
            // e.Value contains the value from the <option value="..."> 
            var value = e.Value?.ToString();

            // Check if the user selected a valid month (not the default "0")
            if (!string.IsNullOrEmpty(value) && value != "0")
            {
                loading = true;
                selectedMonth = value;

                AllHistory =  await purchaseHistoryAC.GetCustomerServiceRecordByMonthAsync(selectedMonth);
                Console.WriteLine($"The user selected: {selectedMonth}");
                loading = false;
                StateHasChanged();
            }
            else
            {
                // Reset if they select the default option
                selectedMonth = "";
            }
        }
    }
}