using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        // 1. Inject the state
        [Inject]
        public IAppointmentDetailState AppointmentState { get; set; } = default!;

        // Hold the data locally for the UI to use
        public AppointmentResponseDTO MyAppointment { get; set; }

        protected override void OnInitialized()
        {
            // 2. Grab the data from the shared state!
            MyAppointment = AppointmentState.SelectedAppointment;

            // 3. Safety check: If the user refreshes the page manually, the state will be lost.
            // If it's null, kick them back to the main appointment page.
            if (MyAppointment == null || MyAppointment.appointmentID != Id)
            {
                navigationManager.NavigateTo("/Appointment");
                return;
            }

            // Note: Since you already have the data in 'MyAppointment', 
            // you don't need to map it to 'AppointmentViewModelDetails' unless you 
            // still need to fetch additional nested data (like Staff names, Services, etc.).
        }

        private void navGoBack()
        {
            if (FromPage == "home") navigationManager.NavigateTo("/home");
            if (FromPage == "Appointment") navigationManager.NavigateTo("/Appointment");
        }
    }
}
