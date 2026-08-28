using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.CompanyDTO
{
    public class ServiceStaffResponseDTO
    {
        public string masterAccountID { get; set; } = string.Empty;

        public string displayCode { get; set; } = string.Empty;

        public string salesPersonCode { get; set; } = string.Empty;

        public string employeeTypeName { get; set; } = string.Empty;

        public string accountName { get; set; } = string.Empty;

        public string jobTitle { get; set; } = string.Empty;

        public string imagePath { get; set; } = string.Empty;
    }
}
