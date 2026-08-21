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
        private IAppointmentService AppointmentService { get; set; } = default!;

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
                try
                {
                    var response = await AppointmentService.GetAppointmentList(
                        DateTime.Now.AddYears(-1),
                        DateTime.Now.AddYears(1));

                    MyAppointment = response?.result?.FirstOrDefault(item => item.appointmentID == Id);
                    if (MyAppointment is not null)
                    {
                        AppointmentState.SetSelectedAppointment(MyAppointment);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[AppointmentDetails] Unable to resolve notification booking: {ex.GetType().Name}");
                }

                if (MyAppointment is null)
                {
                    navigationManager.NavigateTo("/Appointment");
                    return;
                }
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

        private string GetScheduledText(DateTime appointmentDate)
        {
            var days = (appointmentDate.Date - DateTime.Now.Date).Days;
            if (days == 0) return Loc["Today"];
            if (days == 1) return Loc["Tomorrow"];
            if (days > 1) return string.Format(Loc["DaysFromNow"], days);
            return Loc["Passed"];
        }

        private async Task OpenMap()
        {
            var address = BranchDetails == null
                ? ""
                : $"{BranchDetails.address1} {BranchDetails.address2}".Trim();

            if (string.IsNullOrWhiteSpace(address))
            {
                ShowWarningModal(Loc["AddressEmptyTitle"], Loc["AddressEmptyMessage"]);
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
                ShowWarningModal(Loc["PhoneEmptyTitle"], Loc["PhoneEmptyMessage"]);
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
                ShowWarningModal(Loc["WhatsAppEmptyTitle"], Loc["WhatsAppEmptyMessage"]);
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
