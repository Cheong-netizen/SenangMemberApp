using Microsoft.AspNetCore.Components;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSummary
    {
        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;

        [Inject]
        private IServiceProducts productService { get; set; } = default!;

        private List<ServicesModel> servicesDetails = new List<ServicesModel>();
        private bool cancelModalIsOpen = false;
        private bool confirmModalIsOpen = false;

        protected override void OnInitialized()
        {
            if (appointmentState.SelectedServices == null || !appointmentState.SelectedServices.Any())
            {
                var allServices = productService.GetServices() ?? new List<ServicesModel>();
                if (appointmentState.SelectedServiceIds != null && appointmentState.SelectedServiceIds.Any())
                {
                    servicesDetails = allServices.Where(s => appointmentState.SelectedServiceIds.Contains(s.Id)).ToList();
                }
            }
        }

        private void cancelBooking()
        {
            cancelModalIsOpen = true;
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
            navigationManager.NavigateTo("/AppointmentSelectDate");
        }
    }
}
