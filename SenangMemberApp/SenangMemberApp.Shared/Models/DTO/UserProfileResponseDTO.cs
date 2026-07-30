using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class UserProfileResponseDTO
    {
        public bool IsLoading { get; set; }
        public string? Phone { get; set; }
        public string? AccountName { get; set; }
        public string? MemberPassword { get; set; }
        public int BirthdayYear { get; set; }
        public int BirthdayMonth { get; set; }
        public int BirthdayDay { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? AvatarUri { get; set; }
        public bool IsActive { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenTimeSpan { get; set; }
        public string? SaveAction { get; set; }
        public bool IsDirty { get; set; }
    }
}
