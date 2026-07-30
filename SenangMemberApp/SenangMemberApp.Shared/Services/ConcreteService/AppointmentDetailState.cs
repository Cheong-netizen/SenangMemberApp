using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Services.IService;
using System;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class AppointmentDetailState : IAppointmentDetailState
    {
        public AppointmentResponseDTO SelectedAppointment { get; private set; }

        public event Action OnStateChange;
        private void NotifyStateChanged() => OnStateChange?.Invoke();

        public void SetSelectedAppointment(AppointmentResponseDTO appointment)
        {
            SelectedAppointment = appointment;
            NotifyStateChanged();
        }
    }
}