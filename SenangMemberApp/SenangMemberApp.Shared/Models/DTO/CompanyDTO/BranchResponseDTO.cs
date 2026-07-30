using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.CompanyDTO
{
    public class BranchResponseDTO
    {
        public string branchID { get; set; }

        public string branch { get; set; }

        public string locationID { get; set; }

        public string companyName { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public string imagePath { get; set; }

        public bool isShownInApp { get; set; }
    }
}
