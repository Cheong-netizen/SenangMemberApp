//using SenangMemberApp.Shared.Models;
//using SenangMemberApp.Shared.Repositories.IRepository;
//using SenangMemberApp.Shared.Services.ConcreteService;
//using SenangMemberApp.Shared.Services.IService;
//using Microsoft.AspNetCore.Components;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SenangMemberApp.Shared.Pages.AppointmentPages
//{
//    public partial class AppointmentSelectStaff
//    {
//        private List<StaffModel> staffs = new List<StaffModel>();
//        private int selectedStaffId = 0;
//        private int OutletId;
//        private bool loaded = false;
//        [Inject]
//        private NavigationManager navigationManager { get; set; } = default!;
//        [Inject]
//        private IAppointmentState appointmentState { get; set; } = default!;
//        [Inject]
//        private IStoreService storeService { get; set; } = default!;

//        private void navSelectDate()
//        {
//            if (selectedStaffId < 1)
//                return;

//            appointmentState.SetStaff(selectedStaffId);
//            navigationManager.NavigateTo("/AppointmentSelectDate");
//        }
//        protected override void OnInitialized()
//        {
//            OutletId = appointmentState.CurrentAppointment.OutletId;
//            staffs = storeService.GetStaffsByOutletId(OutletId);
//        }

//        protected override void OnAfterRender(bool firstRender)
//        {
//            if (firstRender)
//            {
//                loaded = true;
//                StateHasChanged();
//            }
//        }

//        private void selectStaff(int staffId)
//        {
//            selectedStaffId = staffId;
//        }
//        private void navBack()
//        {
//            navigationManager.NavigateTo("/AppointmentSelectServices");
//        }
//    }
//}
