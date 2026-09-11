using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Models.DTO.PurchaseHistoryDTO;
using SenangMemberApp.Shared.Pages.AppointmentPages;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Home : IDisposable
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
        private bool isAccountNotExistModalVisible = false;

        [Parameter]
        [SupplyParameterFromQuery(Name = "accountDeleted")]
        public bool? AccountDeleted { get; set; }

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
        ITokenService tokenService { get; set; } = default!;
        [Inject]
        IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        IUrlLauncher UrlLauncher { get; set; } = default!;
        [Inject]
        PurchaseHistoryAC purchaseHistoryAC { get; set; } = default!;
        [Inject]
        IThemeService ThemeService { get; set; } = default!;

        private bool isColorPickerVisible = false;
        private List<TodayBillResponseDTO> todayBills = new();
        private bool isTodayBillLoading = false;
        private bool isReviewModalOpen = false;
        private TodayBillResponseDTO? selectedReviewBill;
        private List<AppointmentResponseDTO> allUpcomingAppointments = new();
        private List<BranchResponseDTO> currentBranches = new();

        protected override async Task OnInitializedAsync()
        {
            ShopState.OnStateChange += OnShopStateChanged;
            loading = true;
            services = ServiceProducts.GetServices();
            await LoadCompanyData();
            currentShopId = ShopState.CurrentShopId;
            currentShopName = ShopState.CurrentShopName;

            bool isDeletedFromQuery = AccountDeleted == true || navManager.Uri.Contains("accountDeleted=true", StringComparison.OrdinalIgnoreCase);

            var response = await userProfile.GetUserProfile();
            if (response != null && response.statusCode == 200 && response.result != null)
            {
                userProfileData = response.result;
                if (!userProfileData.IsActive || isDeletedFromQuery)
                {
                    isAccountNotExistModalVisible = true;
                }
            }
            else if (isDeletedFromQuery)
            {
                isAccountNotExistModalVisible = true;
            }

            await LoadAppointmentData();
            await LoadTodayBillData();
            loading = false;
            StateHasChanged();
        }

        private void OnShopStateChanged()
        {
            InvokeAsync(async () =>
            {
                currentShopId = ShopState.CurrentShopId;
                currentShopName = ShopState.CurrentShopName;
                await LoadAppointmentData();
                await LoadTodayBillData();
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            ShopState.OnStateChange -= OnShopStateChanged;
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
            await LoadTodayBillData();
            loading = false;
        }

        public void navAppointment()
        {
            navManager.NavigateTo("/Appointment");
        }

        public void navSelectCompany()
        {
            navManager.NavigateTo("/select-company");
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
            todayBills = new();
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

        private async Task HandleAccountNotExistLogout()
        {
            isAccountNotExistModalVisible = false;
            await ShopState.ResetStateAsync();
            await tokenService.ClearAsync();
            navManager.NavigateTo("/", replace: true);
        }

        private async Task LoadTodayBillData()
        {
            try
            {
                currentShopId = ShopState.CurrentShopId;
                if (string.IsNullOrEmpty(currentShopId) || currentShopId == "0")
                {
                    todayBills = new();
                    return;
                }

                isTodayBillLoading = true;
                var response = await purchaseHistoryAC.GetCustomerTodayBill();
                if (response != null && response.statusCode == 200 && response.result != null && response.result.Any())
                {
                    todayBills = response.result
                        .Where(b => !string.IsNullOrEmpty(b.documentID))
                        .OrderByDescending(b => b.financialDate)
                        .ToList();
                }
                else
                {
                    todayBills = new();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HOME] Failed to load today bills: {ex.Message}");
                todayBills = new();
            }
            finally
            {
                isTodayBillLoading = false;
                StateHasChanged();
            }
        }

        private void OpenReview(TodayBillResponseDTO bill)
        {
            if (bill == null) return;
            selectedReviewBill = bill;
            isReviewModalOpen = true;
            StateHasChanged();
        }

        private void CloseReviewModal()
        {
            isReviewModalOpen = false;
            selectedReviewBill = null;
            StateHasChanged();
        }

        private string GetBillBranchName(TodayBillResponseDTO? bill)
        {
            if (bill == null) return "";
            if (!string.IsNullOrWhiteSpace(bill.branch)) return bill.branch;
            if (!string.IsNullOrWhiteSpace(bill.branchID))
            {
                var branch = GetBranchName(bill.branchID);
                if (!string.IsNullOrWhiteSpace(branch)) return branch;
            }
            return !string.IsNullOrWhiteSpace(ShopState.CurrentShopName) && ShopState.CurrentShopName != "Select Shop"
                ? ShopState.CurrentShopName
                : "";
        }

        private string GetBillItemName(TodayBillResponseDTO? bill)
        {
            if (bill == null) return "";
            if (!string.IsNullOrWhiteSpace(bill.itemName)) return bill.itemName;
            return !string.IsNullOrWhiteSpace(bill.displayCode) ? $"Bill #{bill.displayCode}" : "";
        }

        private string? GetBillGoogleReviewPlaceId(TodayBillResponseDTO? bill)
        {
            if (bill == null) return null;
            if (!string.IsNullOrWhiteSpace(bill.branchID))
            {
                var branch = currentBranches?.FirstOrDefault(b => b.branchID == bill.branchID);
                if (branch != null && !string.IsNullOrWhiteSpace(branch.googleReviewPlaceID))
                {
                    return branch.googleReviewPlaceID;
                }
            }
            return currentBranches?.FirstOrDefault(b => !string.IsNullOrWhiteSpace(b.googleReviewPlaceID))?.googleReviewPlaceID;
        }

        private string GetCustomerId(TodayBillResponseDTO? bill)
        {
            if (bill != null && !string.IsNullOrWhiteSpace(bill.customerID)) return bill.customerID;
            if (bill != null && !string.IsNullOrWhiteSpace(bill.accountID)) return bill.accountID;
            if (!string.IsNullOrWhiteSpace(userProfileData?.Phone)) return userProfileData.Phone;
            return "";
        }

        private void OpenColorPicker()
        {
            isColorPickerVisible = true;
        }

        private void HandleColorSelected(string color)
        {
            StateHasChanged();
        }
    }
}