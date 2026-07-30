using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SenangMemberApp.Shared.Models.DTO.CreditDTO
{
    public class CreditResponseDTO
    {
        public bool isLoading { get; set; }
        public string arapOutstandingID { get; set; }
        public string accountID { get; set; }
        public DateTime financialDate { get; set; }
        public DateTime dueDate { get; set; }
        public string documentID { get; set; }
        public string displayCode { get; set; }
        public int documentTypeID { get; set; }
        public string documentTypeName { get; set; }
        public string documentLineID { get; set; }
        public object sourceARAPOutstandingID { get; set; }
        public string itemDescription { get; set; }
        public string currencyID { get; set; }
        public string currencyName { get; set; }
        public double exchangeRate { get; set; }
        public int convertedAmount { get; set; }
        public double interOutletRatio { get; set; }
        public double interOutletAmount { get; set; }
        public object mgmTier { get; set; }
        public double totalAmount { get; set; }
        public string branchID { get; set; }
        public string groupID { get; set; }
        public string lineItemID { get; set; }
        public string memberTypeID { get; set; }
        public bool isOpeningBalance { get; set; }
        public object posReceiptLineID { get; set; }
        public object firstExtensionBy { get; set; }
        public DateTime firstExtensionDate { get; set; }
        public int firstExtensionDays { get; set; }
        public object secondExtensionBy { get; set; }
        public DateTime secondExtensionDate { get; set; }
        public int secondExtensionDays { get; set; }
        public object lastExtendedBy { get; set; }
        public DateTime lastExtensionDate { get; set; }
        public string saveAction { get; set; }
        public bool isDirty { get; set; }
        public int amountUtilised { get; set; }
        public double balanceCredit { get; set; }
        public int posReceiptAppliedAmount { get; set; }
        public int memberDiscountPercentage { get; set; }
        public string customerName { get; set; }
        public string customerCode { get; set; }
        public string phone { get; set; }
        public bool isRedeemable { get; set; }
        public double netBalanceAfterUtilised { get; set; }
        public double totalFreeCredit { get; set; }
        public int totalUtilisedCredit { get; set; }
        public int totalUtilisedFreeCredit { get; set; }
        public double balanceFreeCredit { get; set; }
        public int totalUtilisedActualValue { get; set; }
        public double balanceActualValue { get; set; }
        public bool isSelected { get; set; }
    }
}
