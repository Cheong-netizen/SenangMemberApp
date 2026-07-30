using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.LoginDTO
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public bool requiresTwoFactor { get; set; }
        public bool requiresCompanySelection { get; set; }
        public string accessToken { get; set; }
        public string message { get; set; }
    }
}
