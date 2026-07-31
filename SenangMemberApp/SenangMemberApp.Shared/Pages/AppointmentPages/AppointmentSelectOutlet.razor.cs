using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.ConcreteService;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSelectOutlet
    {
        [Inject]
        private NavigationManager navManager { get; set; } = default!;
        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;
        [Inject]
        private ICompanyService companyService { get; set; } = default!;
        private int shopId;
        private ShopModel? currentShop = new();
        private int selectedOutletId = 0;
        private string outletSearchText = string.Empty;
        private List<BranchResponseDTO> outletList = new();
        private IEnumerable<BranchResponseDTO> filteredOutlets
        {
            get
            {
                var outlets = outletList;

                if (outlets == null)
                    return Enumerable.Empty<BranchResponseDTO>();

                if (string.IsNullOrWhiteSpace(outletSearchText))
                {
                    return outlets;
                }
                else
                {
                    return outlets.Where(s => s != null && s.branch != null && s.branch.Contains(outletSearchText, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
        protected override async Task OnInitializedAsync()
        {
            //shopId = appointmentState.CurrentAppointment.ShopId;

            if (appointmentState.selectedBookingShopId == "0")
            {
                navManager.NavigateTo("/AppointmentSelectShop");
                return;
            }

            var response = await companyService.GetCompanyBranchDetails();

            outletList = response?.result ?? new List<BranchResponseDTO>();
        }

        private void selectOutlet(BranchResponseDTO outlet)
        {
            appointmentState.SetOutlet(outlet.branchID);
            appointmentState.SetOutlet(outlet);
        }
        private void navSelectServices()
        {
            if(appointmentState.selectedOutletId != "")
            {
                navManager.NavigateTo("/AppointmentSelectDate");
            }
        }
        private void navBack()
        {
            navManager.NavigateTo("/AppointmentSelectShop");
        }
    }
}