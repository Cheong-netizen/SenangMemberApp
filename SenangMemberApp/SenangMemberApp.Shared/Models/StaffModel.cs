using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class StaffModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ShopId { get; set; }
        public int OutletId { get; set; }
        public List<BlockedRange> OccupiedTimeRange { get; set; } = new();
    }
}
