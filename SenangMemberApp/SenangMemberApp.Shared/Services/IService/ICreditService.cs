using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface ICreditService
    {
        Task<ApiResponseRoot<CreditResponseDTO>> FetchCreditBalance();
    }
}
