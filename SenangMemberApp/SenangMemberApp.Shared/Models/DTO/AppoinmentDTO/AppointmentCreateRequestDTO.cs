using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.AppoinmentDTO
{
    public class AppointmentCreateRequestDTO
    {
        public string locationBranchId { get; set; }
        public string appointmentDate { get; set; }
        public string memo { get; set; }
        public string staffId { get; set; }
        public string serviceId { get; set; }
    }
}
