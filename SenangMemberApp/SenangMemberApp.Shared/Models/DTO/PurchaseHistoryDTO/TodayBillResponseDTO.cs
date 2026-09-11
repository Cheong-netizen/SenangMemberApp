using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.PurchaseHistoryDTO
{
    public class TodayBillResponseDTO
    {
        public string documentID { get; set; }
        public string displayCode { get; set; }
        public DateTime financialDate { get; set; }
        public string accountID { get; set; }
        public string branchID { get; set; }
        public string groupID { get; set; }
        public decimal totalAfterTax { get; set; }
        public string? itemName { get; set; }
        public string? branch { get; set; }
        public string? reviewUrl { get; set; }
    }
}
