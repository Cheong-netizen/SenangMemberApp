using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.LoginDTO
{
    public class AuthResponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }

    public class LoginResponseDTO
    {
        public bool requiresTwoFactor { get; set; }
        public bool requiresCompanySelection { get; set; }
        public AuthResponse authResponse { get; set; }
    }
}
