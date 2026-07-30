using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.LoginDTO
{
    public class RegisterRequestDTO
    {
        public string MemberName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
