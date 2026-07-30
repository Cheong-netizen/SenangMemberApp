using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SenangMemberApp.Shared.ApiClient
{
    public class CreditBalanceAC : BaseAC
    {
        public CreditBalanceAC(HttpClient httpClient, ITokenService tokenService) : base(httpClient, tokenService)
        {
        }

        public async Task<ApiResponseRoot<CreditResponseDTO>> FetchCreditBalance()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<CreditResponseDTO>>(
                "api/PublicMember/GetCreditBalanceDetails",
                new { }
            );

            Debug.WriteLine($"API Response: StatusCode={response?.statusCode}, Message={response?.message}, Result={response?.result}");

            return response;
        }
    }
}
