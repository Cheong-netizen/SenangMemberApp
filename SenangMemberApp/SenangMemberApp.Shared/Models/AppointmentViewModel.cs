using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class AppointmentViewModel
    {
        public int id { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public int OutletId { get; set; }
        public string OutletName { get; set; } = string.Empty;
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public HashSet<int> ServicesId { get; set; } = new();
        public List<ServicesModel> Services { get; set; } = new();
        public TimeSpan TimeRequired { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string DaysToGo { get; set; } = string.Empty;
    }
}
