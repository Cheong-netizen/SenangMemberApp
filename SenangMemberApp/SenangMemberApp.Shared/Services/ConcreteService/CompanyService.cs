using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class CompanyService : ICompanyService
    {
        private readonly CompanyAC _companyAC;
        public CompanyService(CompanyAC companyAC)
        {
            _companyAC = companyAC;
        }

        public async Task<ApiResponseRoot<List<CreditResponseDTO>>> GetCompanyCreditBalanceDetails()
        {
            var response = await _companyAC.FetchCompanyCreditBalanceDetails();

            return response;
        }

        public async Task<ApiResponseRoot<List<CompanyResponseDTO>>> GetCompanyList()
        {
            var response = await _companyAC.FetchCompanyList();

            return response;
        }

        public async Task<MemberBalanceDTO> GetCompanyMemberBalance()
        {
            var response = await _companyAC.FetchCompanyMemberBalance();
            return response.result;
        }

        public async Task<ApiResponseRoot<List<PackageResponseDTO>>> GetCompanyPackageDetails()
        {
            var response = await _companyAC.FetchCompanyPackageDetails();

            return response;
        }

        public async Task<ApiResponseRoot<List<BranchResponseDTO>>> GetCompanyBranchDetails()
        {
            var response = await _companyAC.FetchCompanyBranchDetails();
            return response;
        }

        public async Task<ApiResponseRoot<List<BroadcastResponseDTO>>> GetCompanyBroadcast()
        {
            var response = await _companyAC.FetchBroadcastWithinSixMonths();

            return response;
        }
    }
}
