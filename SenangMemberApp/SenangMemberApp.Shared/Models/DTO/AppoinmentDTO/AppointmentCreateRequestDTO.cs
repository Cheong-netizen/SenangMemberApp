using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.AppoinmentDTO
{
    public class AppointmentCreateRequestDTO
    {
        public string? appointmentID { get; set; } = null;
        public string startTime { get; set; } = string.Empty;
        public string branchID { get; set; } = string.Empty;
        public string employeeID { get; set; } = string.Empty;
        public string customerID { get; set; } = string.Empty;
        public string customerName { get; set; } = string.Empty;
        public string memo { get; set; } = string.Empty;
        public string endTime { get; set; } = string.Empty;
        public string inventoryIDs { get; set; } = string.Empty;
        public string saveAction { get; set; } = "Added";
        public bool isDirty { get; set; } = true;
    }
}
