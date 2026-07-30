using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.LoginDTO
{
    public class RegisterResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? SuccessMessage { get; set; }
    }
}
