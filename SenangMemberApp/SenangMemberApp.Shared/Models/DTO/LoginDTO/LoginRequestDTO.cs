using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.LoginRequestDTO
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string email { get; set; } = "";
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; } = "";
        public bool rememberMe { get; set; } = true;
        public string returnUrl { get; set; } = "";
    }
}
