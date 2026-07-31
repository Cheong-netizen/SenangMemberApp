using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Pages.AppointmentPages;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Home
    {
        private bool shopListModalIsOpen = false;
        private bool appointmentMoreModalIsOpen = false;
        private AppointmentResponseDTO? selectedAppointmentForMore;
        private BranchResponseDTO? selectedBranchForMore;
        private List<ServicesModel> services = new();
        private List<ShopModel> shops = new();
        private string shopSearchText = string.Empty;
        private AppointmentModel? upcomingAppointmentToShow = new();
        private AppointmentViewModel? upcomingAppointmentDetailsToShow;
        private string currentShopId;
        private string currentShopName = "";
        private UserProfileResponseDTO userProfileData;
        private bool loading = false;
        private bool warningModalIsOpen = false;
        private string warningModalTitle = "";
        private string warningModalMessage = "";

        private IEnumerable<CompanyResponseDTO> filteredShops // Ensure type matches your list
        {
            get
            {
                if (ShopState.CompanyList == null)
                    return Enumerable.Empty<CompanyResponseDTO>();

                if (string.IsNullOrWhiteSpace(shopSearchText))
                {
                    return ShopState.CompanyList;
                }
                else
                {
                    return ShopState.CompanyList
                        .Where(s => s != null && s.ShopName != null && s.ShopName.Contains(shopSearchText, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        [Inject]
        NavigationManager navManager { get; set; } = default!;
        [Inject]
        IServiceProducts ServiceProducts { get; set; } = default!;
        [Inject]
        IAppointmentService AppointmentService { get; set; } = default!;
        [Inject]
        IShopState ShopState { get; set; } = default!;
        [Inject]
        ICreditService CreditService { get; set; } = default!;
        [Inject]
        ICompanyService companyService { get; set; } = default!;
        [Inject]
        IUserProfileService userProfile { get; set; } = default!;
        [Inject]
        IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        IUrlLauncher UrlLauncher { get; set; } = default!;
        private List<AppointmentResponseDTO> allUpcomingAppointments = new();
        private List<BranchResponseDTO> currentBranches = new();

        protected async override void OnInitialized()
        {
            loading = true;
            services = ServiceProducts.GetServices();
            currentShopId = ShopState.CurrentShopId;
            currentShopName = ShopState.CurrentShopName;
            await LoadCompanyData();
            var response = await userProfile.GetUserProfile();
            if (response != null && response.statusCode == 200 && response.result != null)
            {
                userProfileData = response.result;
            }
            await LoadAppointmentData();
            loading = false;
            StateHasChanged();

            // Initial load of data
        }

        private async Task LoadCompanyData()
        {
            await ShopState.InitializeAsync();
        }

        // 1. Create a reusable method to load data based on current state
        private async Task LoadAppointmentData()
        {
            // Get the current ID (0 or specific shop)
            currentShopId = ShopState.CurrentShopId;
            if (currentShopId != "")
            {
                currentShopName = ShopState.CurrentShopName;
            }

            ApiResponseRoot<List<AppointmentResponseDTO>> response = await AppointmentService.GetAppointmentList(DateTime.Now, DateTime.Now.AddYears(1));

            if (response != null && response.statusCode == 200 && response.result != null)
            {
                allUpcomingAppointments = response.result
                    .OrderBy(x => x.startTime)
                    .ToList();
            }
            else
            {
                allUpcomingAppointments = new List<AppointmentResponseDTO>();
            }

            var branchResponse = await companyService.GetCompanyBranchDetails();
            if (branchResponse != null && branchResponse.statusCode == 200 && branchResponse.result != null)
            {
                currentBranches = branchResponse.result;
            }
            else
            {
                currentBranches = new List<BranchResponseDTO>();
            }

            // Ensure UI updates
            StateHasChanged();
        }

        private string GetBranchName(string appointmentLocation)
        {
            var branch = currentBranches?.FirstOrDefault(b => b.branchID == appointmentLocation);
            return branch != null ? branch.branch : appointmentLocation;
        }

        // 2. Updated selectShop method
        private async Task selectShop(string id, string name)
        {
            // Close the modal
            shopListModalIsOpen = false;
            loading = true;

            // Update the global state
            await ShopState.SetShop(id, name);

            // RELOAD the data using the new ID
            await LoadAppointmentData();
            loading = false;
        }

        public void navAppointment()
        {
            navManager.NavigateTo("/Appointment");
        }

        private void toggleShopListModal()
        {
            shopListModalIsOpen = !shopListModalIsOpen;
        }

        private void ShowWarningModal(string title, string message)
        {
            warningModalTitle = title;
            warningModalMessage = message;
            warningModalIsOpen = true;
        }

        private void CloseWarningModal()
        {
            warningModalIsOpen = false;
        }

        private async Task OpenAppointmentMoreModal(AppointmentResponseDTO appointment)
        {
            selectedAppointmentForMore = appointment;
            
            var response = await companyService.GetCompanyBranchDetails();
            if (response != null && response.statusCode == 200 && response.result != null)
            {
                selectedBranchForMore = response.result.FirstOrDefault(b => b.branchID == appointment.appointmentLocation);
            }
            else
            {
                selectedBranchForMore = null;
            }

            var address = selectedBranchForMore == null
                ? ""
                : $"{selectedBranchForMore.address1} {selectedBranchForMore.address2}".Trim();
            var phone = selectedBranchForMore?.phone ?? "";

            if (string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(phone))
            {
                ShowWarningModal(
                    Loc["NoContactInfoTitle"] ?? "Contact Info Unavailable",
                    Loc["NoContactInfoMessage"] ?? "No contact details (address, phone, or WhatsApp) are available for this branch."
                );
            }
            else
            {
                appointmentMoreModalIsOpen = true;
            }
        }

        private void CloseAppointmentMoreModal()
        {
            appointmentMoreModalIsOpen = false;
        }

        private async Task OpenMap()
        {
            if (selectedBranchForMore != null)
            {
                var address = $"{selectedBranchForMore.address1} {selectedBranchForMore.address2}".Trim();
                if (string.IsNullOrWhiteSpace(address))
                {
                    ShowWarningModal(
                        Loc["AddressEmptyTitle"] ?? "Address Unavailable",
                        Loc["AddressEmptyMessage"] ?? "The address for this shop/branch is not available."
                    );
                }
                else
                {
                    var escapedAddress = Uri.EscapeDataString(address);
                    await UrlLauncher.OpenUrlAsync($"https://maps.google.com/?q={escapedAddress}");
                }
            }
            else
            {
                ShowWarningModal(
                    Loc["AddressEmptyTitle"] ?? "Address Unavailable",
                    Loc["AddressEmptyMessage"] ?? "The address for this shop/branch is not available."
                );
            }
        }

        private async Task OpenCall()
        {
            if (selectedBranchForMore != null && !string.IsNullOrEmpty(selectedBranchForMore.phone))
            {
                var phone = selectedBranchForMore.phone.Replace("-", "").Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "");
                await UrlLauncher.OpenUrlAsync($"tel:{phone}");
            }
            else
            {
                ShowWarningModal(
                    Loc["PhoneEmptyTitle"] ?? "Phone Number Unavailable",
                    Loc["PhoneEmptyMessage"] ?? "The phone number for this shop/branch is not available."
                );
            }
        }

        private async Task OpenWhatsApp()
        {
            if (selectedBranchForMore != null && !string.IsNullOrEmpty(selectedBranchForMore.phone))
            {
                var phone = selectedBranchForMore.phone.Replace("-", "").Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "");
                if (phone.StartsWith("0"))
                {
                    phone = "60" + phone.Substring(1);
                }
                await UrlLauncher.OpenUrlAsync($"https://wa.me/{phone}");
            }
            else
            {
                ShowWarningModal(
                    Loc["WhatsAppEmptyTitle"] ?? "WhatsApp Unavailable",
                    Loc["WhatsAppEmptyMessage"] ?? "The WhatsApp contact details for this shop/branch are not available."
                );
            }
        }

        private void navCatalog()
        {
            navManager.NavigateTo("/catalog");
        }
        private void navAppointmentDetails()
        {
            if (upcomingAppointmentDetailsToShow != null)
            {
                navManager.NavigateTo($"/AppointmentDetails/{upcomingAppointmentDetailsToShow.id}/home");
            }
        }
        private void selectAllShop()
        {
            ShopState.SetShop("0", "Select a shop");
            // Close the modal
            shopListModalIsOpen = false;
            currentShopId = "0";
            currentShopName = "Select a shop";
            // RELOAD the data using the new ID
            StateHasChanged();
        }
        private void navCredit()
        {
            navManager.NavigateTo("/Credits");
        }
        //private void navPoint()
        //{
        //    navManager.NavigateTo("/Points");
        //}
        private void navPackage()
        {
            navManager.NavigateTo("/Packages");
        }

        private void navAnnouncement()
        {
            navManager.NavigateTo("/announcement");
        }
    }
}