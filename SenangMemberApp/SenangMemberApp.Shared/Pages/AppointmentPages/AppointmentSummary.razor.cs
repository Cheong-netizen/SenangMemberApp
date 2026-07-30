using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSummary
    {
        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;
        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;
        private int shopId;
        private ShopModel? shopDetails;
        private int outletId; 
        private OutletModel? outletDetails;
        private StaffModel? staffDetails;
        private HashSet<int> servicesId = new();
        private List<ServicesModel> servicesDetails = new List<ServicesModel>();
        private int staffId;
        private TimeSpan timeRequired;
        private DateTime bookingDate;
        private bool shouldRedirect = false;
        private bool loaded = false;
        private bool cancelModalIsOpen = false;
        private bool confirmModalIsOpen = false;

        //protected override void OnInitialized()
        //{
        //    var appt = appointmentState.CurrentAppointment;

        //    if (appt == null ||
        //        appt.ShopId == 0 ||
        //        appt.StaffId == 0 ||
        //        appt.OutletId == 0 ||
        //        appt.ServicesId == null || appt.ServicesId.Count == 0 ||
        //        appt.AppointmentDateTime == default(DateTime))      
        //    {
                
        //        shouldRedirect = true;
        //        return; 
        //    }

        //    shopId = appointmentState.CurrentAppointment.ShopId;
        //    outletId = appointmentState.CurrentAppointment.OutletId;
        //    servicesId = appointmentState.CurrentAppointment.ServicesId;
        //    staffId = appointmentState.CurrentAppointment.StaffId;
        //    timeRequired = appointmentState.CurrentAppointment.TimeRequired;
        //    bookingDate = appointmentState.CurrentAppointment.AppointmentDateTime;

        //    shopDetails = storeService.GetShopDetails(shopId);
        //    outletDetails = storeService.GetOutletDetails(outletId);
        //    staffDetails = storeService.GetStaffDetails(staffId);

        //    if(shopDetails == null || outletDetails == null || staffDetails == null)
        //    {
        //        shouldRedirect = true;
        //        return;
        //    }

        //    foreach (int id in servicesId)
        //    {
        //        ServicesModel? service = storeService.GetServiceDetails(id);
        //        if (service != null)
        //        {
        //            servicesDetails.Add(service);
        //        }
        //    }
        //}

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    if (firstRender && shouldRedirect)
        //    {
        //        navigationManager.NavigateTo("/appointment");
        //    }
        //    if (firstRender)
        //    {
        //        loaded = true;
        //        StateHasChanged();
        //    }
        //}
        private void cancelBooking()
        {
            cancelModalIsOpen = false;
            navigationManager.NavigateTo("/Appointment");
        }
        private void closeCancelModal()
        {
            cancelModalIsOpen = false;
        }

        private async Task confirmBooking()
        {
            bool success = await appointmentState.ConfirmAppointment();
            if (success)
            {
                confirmModalIsOpen = true;
            }
        }
        private void navAppointment()
        {
            navigationManager.NavigateTo("/Appointment");
        }
        private void navBack()
        {
            navigationManager.NavigateTo("/AppointmentSelectOutlet");
        }
    }
}
