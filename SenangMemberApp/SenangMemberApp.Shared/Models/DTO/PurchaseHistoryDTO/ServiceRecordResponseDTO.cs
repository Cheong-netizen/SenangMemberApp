using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.PurchaseHistoryDTO
{
    public class ServiceRecordResponseDTO
    {
        public string serviceRecordID { get; set; }
        public string recordType { get; set; }
        public DateTime recordDate { get; set; }
        public string customerID { get; set; }
        public string stylish { get; set; }
        public decimal rm { get; set; }
        public string branch { get; set; }
        public string itemName { get; set; }
        public string documentLineID { get; set; }
        public string documentID { get; set; }
        public string displayCode { get; set; }
        public decimal quantity { get; set; }
        public decimal unitPrice { get; set; }
        public decimal subTotal { get; set; }
        public int activityTypeID { get; set; }
        public int inventoryTypeID { get; set; }
        public string activityTypeName { get; set; }
        public string inventoryTypeName { get; set; }
        public string branchID { get; set; }
        public string remarks { get; set; }
    }
}
