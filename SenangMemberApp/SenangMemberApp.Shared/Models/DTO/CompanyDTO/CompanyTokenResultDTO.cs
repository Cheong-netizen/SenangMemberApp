using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.CompanyDTO
{
    public class CompanyTokenResultDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
