using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class OutletModel
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Address3 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string POSCode { get; set; } = string.Empty;
        public List<int> StaffsId { get; set; } = new();
    }
}
