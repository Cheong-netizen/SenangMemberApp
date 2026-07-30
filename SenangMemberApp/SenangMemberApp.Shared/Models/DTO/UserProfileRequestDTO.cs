using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class UserProfileRequestDTO
    {
        public string? Phone { get; set; }
        public string? AccountName { get; set; }
        public string? MemberPassword { get; set; }
        public int? BirthdayYear { get; set; }
        public int? BirthdayMonth { get; set; }
        public int? BirthdayDay { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
    }
}
