using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface ICompanyService
    {
        Task<ApiResponseRoot<List<CompanyResponseDTO>>> GetCompanyList();
        Task<ApiResponseRoot<List<CreditResponseDTO>>> GetCompanyCreditBalanceDetails();
        Task<ApiResponseRoot<List<PackageResponseDTO>>> GetCompanyPackageDetails();
        Task<MemberBalanceDTO> GetCompanyMemberBalance();
        Task<ApiResponseRoot<List<BranchResponseDTO>>> GetCompanyBranchDetails();
        Task<ApiResponseRoot<List<BroadcastResponseDTO>>> GetCompanyBroadcast();
    }
}
