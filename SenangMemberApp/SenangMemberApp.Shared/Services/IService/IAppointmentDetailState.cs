using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using System;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IAppointmentDetailState
    {
        AppointmentResponseDTO SelectedAppointment { get; }
        void SetSelectedAppointment(AppointmentResponseDTO appointment);

        // Optional but good practice: an event to notify components if the state changes
        event Action OnStateChange;
    }
}