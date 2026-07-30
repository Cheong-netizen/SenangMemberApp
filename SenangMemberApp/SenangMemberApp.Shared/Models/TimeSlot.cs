using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class TimeSlot
    {
        public string TimeDisplay { get; set; } = string.Empty;
        public DateTime FullDateTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
