using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class CreditService : ICreditService
    {
        private readonly CreditBalanceAC _creditBalanceAC;
        public CreditService(CreditBalanceAC creditBalanceAC)
        {
            _creditBalanceAC = creditBalanceAC;
        }
        public Task<ApiResponseRoot<CreditResponseDTO>> FetchCreditBalance()
        {
            return _creditBalanceAC.FetchCreditBalance();
        }
    }
}
