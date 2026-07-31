using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IShopState
    {
        string CurrentShopId { get; }
        string CurrentShopName { get; }
        decimal currentPackageBalance { get; }
        List<CompanyResponseDTO> CompanyList { get; }
        Task InitializeAsync(); // Add this
        Task ResetStateAsync();
        Task SetShop(string shopId, string shopName);
        public ApiResponseRoot<List<CreditResponseDTO>> currentCreditDetails { get; }
        public double balanceCredit { get; }
        public MemberBalanceDTO currentMemberBalance { get; }
        event Action OnStateChange;
        bool IsLoading { get; }
    }
}
