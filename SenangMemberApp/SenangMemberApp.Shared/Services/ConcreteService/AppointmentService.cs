using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentAC appointmentAC;
        public AppointmentService(AppointmentAC appointmentAC)
        {
            this.appointmentAC = appointmentAC;
        }
        public async Task<ApiResponseRoot<List<AppointmentResponseDTO>>> GetAppointmentList(DateTime startDate, DateTime endDate)
        {
            var payload = new AppointmentRequestDTO
            {
                StartDate = startDate,
                EndDate = endDate
            };
            var response = await appointmentAC.FetchCompanyAppointments(payload);

            return response;
        }
    }
}
