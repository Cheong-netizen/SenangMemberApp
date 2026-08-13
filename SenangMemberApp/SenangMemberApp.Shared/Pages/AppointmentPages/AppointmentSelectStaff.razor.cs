using Microsoft.AspNetCore.Components;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSelectStaff
    {
        private List<StaffModel> staffs = new List<StaffModel>();
        private int selectedStaffId = 0;
        private string selectedStaffName = string.Empty;
        private bool loaded = false;

        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;

        [Inject]
        private IStaffRepository staffRepository { get; set; } = default!;

        protected override void OnInitialized()
        {
            int outletId = 1;
            if (!string.IsNullOrEmpty(appointmentState.selectedOutletId) && int.TryParse(appointmentState.selectedOutletId, out int parsedId))
            {
                outletId = parsedId;
            }

            staffs = staffRepository.getAllStaffByOutletId(outletId);

            if (staffs == null || !staffs.Any())
            {
                // Fallback to default outlet staff list
                staffs = staffRepository.getAllStaffByOutletId(1) ?? new List<StaffModel>();
            }

            if (!string.IsNullOrEmpty(appointmentState.selectedStaffId) && int.TryParse(appointmentState.selectedStaffId, out int savedStaffId))
            {
                selectedStaffId = savedStaffId;
                selectedStaffName = appointmentState.selectedStaffName;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                loaded = true;
                StateHasChanged();
            }
        }

        private void selectStaff(int staffId, string staffName)
        {
            selectedStaffId = staffId;
            selectedStaffName = staffName;
        }

        private void navSelectDate()
        {
            if (selectedStaffId < 1)
                return;

            appointmentState.SetSelectedStaff(selectedStaffId.ToString(), selectedStaffName);
            navigationManager.NavigateTo("/AppointmentSelectDate");
        }

        private void navBack()
        {
            navigationManager.NavigateTo("/AppointmentSelectServices");
        }
    }
}
