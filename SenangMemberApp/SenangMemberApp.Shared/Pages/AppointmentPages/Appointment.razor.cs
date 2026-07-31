using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.ConcreteService;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class Appointment
    {
        [Inject]
        NavigationManager navManager { get; set; } = default!;

        [Inject]
        IServiceProducts ServiceProducts { get; set; } = default!;
        [Inject]
        IAppointmentService AppointmentService { get; set; } = default!;
        [Inject]
        IShopState ShopState { get; set; } = default!;
        [Inject]
        IAppointmentDetailState AppointmentState { get; set; } = default!;
        [Inject]
        ICompanyService companyService { get; set; } = default!;
        [Inject]
        IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        IUrlLauncher UrlLauncher { get; set; } = default!;

        private bool isUpcoming = true;
        private bool shopListModalIsOpen = false;
        private bool appointmentMoreModalIsOpen = false;
        private AppointmentResponseDTO? selectedAppointmentForMore;
        private BranchResponseDTO? selectedBranchForMore;
        private bool warningModalIsOpen = false;
        private string warningModalTitle = "";
        private string warningModalMessage = "";
        //private List<AppointmentModel> appointments = new();
        private IEnumerable<CompanyResponseDTO> companies => ShopState.CompanyList;
        //private List<AppointmentViewModel> upcomingAppointments = new();
        //private List<AppointmentViewModel> pastAppointments = new();
        //private List<AppointmentViewModel> appointmentViewModels = new();
        // Change the flag to represent the 1-month view
        private bool isOneMonthView = true;
        private List<AppointmentResponseDTO> allUpcomingAppointments = new();
        private List<BranchResponseDTO> currentBranches = new();
        private IEnumerable<AppointmentResponseDTO> currentDisplayAppointments
        {
            get
            {
                if (isOneMonthView)
                {
                    // Show only appointments up to 1 month from now
                    var oneMonthFromNow = DateTime.Now.AddMonths(1);
                    return allUpcomingAppointments.Where(x => x.startTime <= oneMonthFromNow);
                }
                else
                {
                    // Show the full 1-year list
                    return allUpcomingAppointments;
                }
            }
        }
        private string shopSearchText = string.Empty;
        private string selectedShopToView = "0";
        private string currentShopName = "Select Shop";
        private bool loadingAppointment = true;
        private IEnumerable<CompanyResponseDTO> filteredShops
        {
            get
            {
                var list = companies ?? ShopState.CompanyList;
                if (list == null)
                    return Enumerable.Empty<CompanyResponseDTO>();

                if (string.IsNullOrWhiteSpace(shopSearchText))
                {
                    return list;
                }
                else
                {
                    return list.Where(s => s != null && s.ShopName != null && s.ShopName.Contains(shopSearchText, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            ShopState.OnStateChange += OnShopStateChanged;
            if (!ShopState.IsLoading)
            {
                await setupCurrentShop();
                await setupAppointmentsData();
            }
            else if (ShopState.CompanyList == null || ShopState.CompanyList.Count == 0)
            {
                // Safety net: Kick off initialization if nothing else in the app has started it yet
                await ShopState.InitializeAsync();
            }

        }

        private async Task setupAppointmentsData()
        {
            loadingAppointment = true;

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

            loadingAppointment = false;
        }

        private string GetBranchName(string appointmentLocation)
        {
            var branch = currentBranches?.FirstOrDefault(b => b.branchID == appointmentLocation);
            return branch != null ? branch.branch : appointmentLocation;
        }

        //private void setupCompanyList()
        //{
        //    companies = ShopState.CompanyList;
        //}

        private async Task setupCurrentShop()
        {
            if (string.IsNullOrEmpty(ShopState.CurrentShopId) || ShopState.CurrentShopId == "0")
            {
                if (ShopState.CompanyList != null && ShopState.CompanyList.Any())
                {
                    var firstShop = ShopState.CompanyList.First();

                    await ShopState.SetShop(firstShop.CompanyCode, firstShop.ShopName);

                    selectedShopToView = firstShop.CompanyCode;
                    currentShopName = firstShop.ShopName;
                }
                else
                {
                    selectedShopToView = "0";
                    currentShopName = "No Shop";
                }
            }
            else
            {
                selectedShopToView = ShopState.CurrentShopId;
                currentShopName = ShopState.CurrentShopName;
            }
        }

        public void navSelectShop()
        {
            navManager.NavigateTo("/AppointmentSelectShop");
        }

        private void toggleOneMonth()
        {
            isOneMonthView = true;
        }

        private void toggleOneYear()
        {
            isOneMonthView = false;
        }

        public void navAppointment()
        {
            navManager.NavigateTo("/Appointment");
        }

        public void navChatBot()
        {
            navManager.NavigateTo("/ChatBot");
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

        private void navAppointmentDetails(AppointmentResponseDTO data)
        {
            // 2. Save the full object to the shared state
            AppointmentState.SetSelectedAppointment(data);

            // 3. Navigate! (We still pass the ID in the URL for clean routing, 
            // but we won't actually need to use it to fetch data)
            navManager.NavigateTo($"/AppointmentDetails/{data.appointmentID}/Appointment");
        }
        private async Task selectShop(string id, string name)
        {
            shopListModalIsOpen = false; // Close UI immediately for better UX
            await ShopState.SetShop(id, name);
        }
        //private async Task selectAllShop()
        //{
        //    await ShopState.SetShop("0", "All Shops");
        //    shopListModalIsOpen = false;
        //}
        private async Task refreshPage()
        {
            //setupCompanyList();
            await setupCurrentShop();
            await setupAppointmentsData();
            StateHasChanged();
        }
        private async void OnShopStateChanged()
        {
            if (!ShopState.IsLoading)
            {
                // InvokeAsync ensures the UI thread safely handles the state change
                await InvokeAsync(async () =>
                {
                    await refreshPage();
                });
            }
            else
            {
                // If it's still loading, just re-render to show a loading spinner
                await InvokeAsync(StateHasChanged);
            }
        }
        public void Dispose()
        {
            ShopState.OnStateChange -= OnShopStateChanged;
        }
    }
}