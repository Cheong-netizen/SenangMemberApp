using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class BlockedRange
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
