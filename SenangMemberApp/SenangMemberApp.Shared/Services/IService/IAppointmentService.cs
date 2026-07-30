using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IAppointmentService
    {
        public Task<ApiResponseRoot<List<AppointmentResponseDTO>>> GetAppointmentList(DateTime startDate, DateTime endDate);
    }
}
