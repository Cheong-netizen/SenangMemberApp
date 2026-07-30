using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class BroadcastResponseDTO
    {
        public bool IsLoading { get; set; }
        public string? NewsID { get; set; }
        public DateTime FinancialDate { get; set; }
        public string? ImageFileName { get; set; }
        public string? MessageHeader { get; set; }
        public string? MessageBody { get; set; }
        public string? BranchID { get; set; }
        public string? MessageHtml { get; set; }
        public string? NewsType { get; set; }
        public string? HeaderImageFileName { get; set; }
        public string? ShortMessageBody { get; set; }
        public string? SaveAction { get; set; }
        public bool IsDirty { get; set; }
    }
}
