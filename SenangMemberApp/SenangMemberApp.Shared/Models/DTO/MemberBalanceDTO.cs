using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class MemberBalanceDTO
    {
        public decimal packageBalance { get; set; }

        public decimal creditBalance { get; set; }

        public decimal pointBalance { get; set; }

        public decimal pointRebateBalance { get; set; }

        public decimal voucherBalance { get; set; }
    }
}
