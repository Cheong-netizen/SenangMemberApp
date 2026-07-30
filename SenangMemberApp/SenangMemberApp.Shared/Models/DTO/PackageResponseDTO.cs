using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class PackageResponseDTO
    {
        public string autoID { get; set; }
        public string packageName { get; set; }
        public decimal quantityAvailable { get; set; }
        public DateTime expiryDate { get; set; }
        public DateTime financialDate { get; set; }
        public string displayCode { get; set; }
        public string description { get; set; }
        public decimal quantityPurchased { get; set; }
        public decimal quantityRedeemed { get; set; }
        public decimal quantityUtilised { get; set; }
        public decimal netBalanceAfterUtilised { get; set; }
        public string customerName { get; set; }
        public bool isRedeemable { get; set; }
    }
}
