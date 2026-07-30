using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class ServiceResponseDTO
    {
        public string masterAccountID { get; set; }

        public int inventoryTypeID { get; set; }

        public string inventoryTypeName { get; set; }

        public string displayCode { get; set; }

        public string salesDescription { get; set; }

        public string itemBrandID { get; set; }

        public string itemBrandName { get; set; }

        public string itemGroupID { get; set; }

        public string itemGroupName { get; set; }

        public decimal salesPrice { get; set; }

        public string accountStatus { get; set; }

        public string imageFileName { get; set; }

        public string imagePath { get; set; }

        public string taxCodeID { get; set; }

        public bool isTaxInclusive { get; set; }

        public string taxTypeID { get; set; }

        public decimal taxRate { get; set; }

        public string taxFinancialAccountID { get; set; }

        public string remarks { get; set; }
    }
}
