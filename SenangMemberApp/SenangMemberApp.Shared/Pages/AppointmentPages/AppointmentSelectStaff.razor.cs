using Microsoft.AspNetCore.Components;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSelectStaff
    {
        private List<ServiceStaffResponseDTO> staffs = new();
        private string selectedStaffId = string.Empty;
        private string selectedStaffName = string.Empty;
        private bool isLoading = true;

        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;

        [Inject]
        private ICompanyService companyService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            try
            {
                var branchCode = appointmentState.selectedOutlet?.branchID;
                if (string.IsNullOrWhiteSpace(branchCode))
                {
                    branchCode = !string.IsNullOrWhiteSpace(appointmentState.selectedOutletId)
                        ? appointmentState.selectedOutletId
                        : "hq";
                }

                var response = await companyService.GetCompanyServiceStaffByBranch(branchCode);
                staffs = response?.result ?? new List<ServiceStaffResponseDTO>();

                if (!string.IsNullOrEmpty(appointmentState.selectedStaffId))
                {
                    selectedStaffId = appointmentState.selectedStaffId;
                    selectedStaffName = appointmentState.selectedStaffName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppointmentSelectStaff Error] {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void selectStaff(string staffId, string staffName)
        {
            selectedStaffId = staffId;
            selectedStaffName = staffName;
        }

        private void navSelectDate()
        {
            if (string.IsNullOrWhiteSpace(selectedStaffId))
                return;

            appointmentState.SetSelectedStaff(selectedStaffId, selectedStaffName);
            navigationManager.NavigateTo("/AppointmentSelectDate");
        }

        private void navBack()
        {
            navigationManager.NavigateTo("/AppointmentSelectServices");
        }

        private string GetStaffImageUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.StartsWith("C:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("D:", StringComparison.OrdinalIgnoreCase))
            {
                return "_content/SenangMemberApp.Shared/Images/store_placeholder.png";
            }
            return path;
        }
    }
}
