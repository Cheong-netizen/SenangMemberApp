using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class AppointmentModel
    {
        public int id { get; set; }
        public int ShopId { get; set; }
        public int OutletId { get; set; }
        public int StaffId { get; set; }
        public HashSet<int> ServicesId { get; set; } = new();
        public TimeSpan TimeRequired { get; set; }
        public DateTime AppointmentDateTime { get; set; }
    }
}
