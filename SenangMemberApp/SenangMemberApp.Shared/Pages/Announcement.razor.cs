using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Announcement
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        ICompanyService CompanyService { get; set; } = default!;

        [Inject]
        public IShopState ShopState { get; set; } = default!;

        private List<BroadcastResponseDTO> Announcements { get; set; } = new List<BroadcastResponseDTO>();

        // UI State
        private bool shopListModalIsOpen = false;
        private string shopSearchText = string.Empty;

        // Data State
        private string currentShopId = string.Empty;
        private string currentShopName = string.Empty;

        // FIXED: Changed type from ShopModel to CompanyResponseDTO
        private List<CompanyResponseDTO> shops = new();

        private bool loading = false;

        // FIXED: Changed type from ShopModel to CompanyResponseDTO
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

        protected override async Task OnInitializedAsync()
        {
            loading = true;
            var response = await CompanyService.GetCompanyBroadcast();

            if (response != null)
            {
                Announcements = response.result ?? new List<BroadcastResponseDTO>();
            }

            // 2. Populate the local list using the correct DTO type
            shops = ShopState.CompanyList ?? new List<CompanyResponseDTO>();

            // 3. Set the currently selected shop UI
            currentShopId = ShopState.CurrentShopId;
            currentShopName = ShopState.CurrentShopName;
            loading = false;
        }

        private void navBack()
        {
            NavManager.NavigateTo("/home");
        }

        private void toggleShopListModal()
        {
            shopListModalIsOpen = !shopListModalIsOpen;

            if (!shopListModalIsOpen)
            {
                shopSearchText = string.Empty;
            }
        }

        private async Task selectShop(string id, string name)
        {
            shopListModalIsOpen = false;
            loading = true;
            await ShopState.SetShop(id, name);
            currentShopId = id;
            currentShopName = name;
            
            Announcements = new List<BroadcastResponseDTO>();

            var response = await CompanyService.GetCompanyBroadcast();

            if (response != null)
            {
                Announcements = response.result ?? new List<BroadcastResponseDTO>();
            }
            loading = false;
        }

        private void selectAllShop()
        {
            ShopState.SetShop("0", "Select a shop");
            currentShopId = "0";
            currentShopName = "Select a shop";
            shopListModalIsOpen = false;
            Announcements = new List<BroadcastResponseDTO>();
        }

        private string GetTimeDisplay(DateTime date)
        {
            var diff = DateTime.Now - date;
            if (diff.TotalMinutes < 1) return "Just now";
            if (diff.TotalMinutes < 60) return $"{diff.Minutes}m";
            if (diff.TotalHours < 24) return $"{diff.Hours}h";
            return date.ToString("dd MMM");
        }

        private void OpenMessage(MessageModel msg)
        {
            msg.IsRead = true;
        }

        public class MessageModel
        {
            public int Id { get; set; }
            public string SenderName { get; set; } = "";
            public string Body { get; set; } = "";
            public DateTime SentDate { get; set; }
            public bool IsRead { get; set; }
        }
    }
}