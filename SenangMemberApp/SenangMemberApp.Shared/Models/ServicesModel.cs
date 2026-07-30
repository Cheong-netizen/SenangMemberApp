using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models
{
    public class ServicesModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = String.Empty;
        public string CategoryName { get; set; } = String.Empty;
        public int Price { get; set; }
        public string Description { get; set; } = String.Empty;
        public int ShopId { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
    }
}
