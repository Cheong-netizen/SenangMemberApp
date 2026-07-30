using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class DaySchedule
    {
        public DateTime Date { get; set; }
        public bool IsOpen { get; set; }
        public List<TimeSlot> TimeSlots { get; set; } = new();
    }
}
