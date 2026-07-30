using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.AppoinmentDTO
{
    public class AppointmentResponseDTO
    {
        public string appointmentID { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string memo { get; set; }
        public string customerName { get; set; }
        public string branchID { get; set; }
        public string appointmentLocation { get; set; }
        public DateTime createdDateTime { get; set; }
    }
}
