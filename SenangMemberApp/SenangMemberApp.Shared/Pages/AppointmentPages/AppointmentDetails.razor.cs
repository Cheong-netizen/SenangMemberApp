using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentDetails
    {
        [Parameter]
        public string Id { get; set; } // Changed to string to match DTO

        [Parameter, EditorRequired]
        public string FromPage { get; set; }

        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        public IAppointmentDetailState AppointmentState { get; set; } = default!;

        [Inject]
        private ICompanyService companyService { get; set; } = default!;

        [Inject]
        private IShopState ShopState { get; set; } = default!;

        [Inject]
        private IUrlLauncher UrlLauncher { get; set; } = default!;

        public AppointmentResponseDTO MyAppointment { get; set; }
        public BranchResponseDTO? BranchDetails { get; set; }

        public string StoreImageUrl { get; set; } = "_content/SenangMemberApp.Shared/Images/store_placeholder.png";

        public string WarningModalTitle { get; set; } = "";
        public string WarningModalMessage { get; set; } = "";
        public bool WarningModalIsOpen { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            MyAppointment = AppointmentState.SelectedAppointment;

            if (MyAppointment == null || MyAppointment.appointmentID != Id)
            {
                navigationManager.NavigateTo("/Appointment");
                return;
            }

            try
            {
                var response = await companyService.GetCompanyBranchDetails();
                if (response != null && response.statusCode == 200 && response.result != null)
                {
                    BranchDetails = response.result.FirstOrDefault(b =>
                        b.branchID == MyAppointment.appointmentLocation ||
                        b.branchID == MyAppointment.branchID);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppointmentDetails] Error fetching branch details: {ex.Message}");
            }

            SetStoreImageUrl();
        }

        private void SetStoreImageUrl()
        {
            if (BranchDetails != null && !string.IsNullOrWhiteSpace(BranchDetails.imagePath))
            {
                StoreImageUrl = BranchDetails.imagePath;
            }
            else
            {
                var matchedCompany = ShopState.CompanyList?.FirstOrDefault(c =>
                    c.ShopName == ShopState.CurrentShopName || c.CompanyCode == ShopState.CurrentShopId);

                if (matchedCompany != null && !string.IsNullOrWhiteSpace(matchedCompany.LogoPath))
                {
                    StoreImageUrl = matchedCompany.LogoPath;
                }
                else
                {
                    StoreImageUrl = "_content/SenangMemberApp.Shared/Images/store_placeholder.png";
                }
            }
        }

        private void HandleImageError()
        {
            StoreImageUrl = "_content/SenangMemberApp.Shared/Images/store_placeholder.png";
        }

        private async Task OpenMap()
        {
            var address = BranchDetails == null
                ? ""
                : $"{BranchDetails.address1} {BranchDetails.address2}".Trim();

            if (string.IsNullOrWhiteSpace(address))
            {
                ShowWarningModal("Address Unavailable", "The address for this shop/branch is not available.");
            }
            else
            {
                var escapedAddress = Uri.EscapeDataString(address);
                await UrlLauncher.OpenUrlAsync($"https://maps.google.com/?q={escapedAddress}");
            }
        }

        private async Task OpenCall()
        {
            if (BranchDetails != null && !string.IsNullOrEmpty(BranchDetails.phone))
            {
                var phone = BranchDetails.phone.Replace("-", "").Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "");
                await UrlLauncher.OpenUrlAsync($"tel:{phone}");
            }
            else
            {
                ShowWarningModal("Phone Number Unavailable", "The phone number for this shop/branch is not available.");
            }
        }

        private async Task OpenWhatsApp()
        {
            if (BranchDetails != null && !string.IsNullOrEmpty(BranchDetails.phone))
            {
                var phone = BranchDetails.phone.Replace("-", "").Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "");
                if (phone.StartsWith("0"))
                {
                    phone = "60" + phone.Substring(1);
                }
                await UrlLauncher.OpenUrlAsync($"https://wa.me/{phone}");
            }
            else
            {
                ShowWarningModal("WhatsApp Unavailable", "The WhatsApp contact details for this shop/branch are not available.");
            }
        }

        private void ShowWarningModal(string title, string message)
        {
            WarningModalTitle = title;
            WarningModalMessage = message;
            WarningModalIsOpen = true;
        }

        private void CloseWarningModal()
        {
            WarningModalIsOpen = false;
        }

        private void navGoBack()
        {
            if (FromPage == "home") navigationManager.NavigateTo("/home");
            else navigationManager.NavigateTo("/Appointment");
        }
    }
}
