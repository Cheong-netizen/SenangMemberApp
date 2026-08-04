using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Models.DTO.CreditDTO;
using SenangMemberApp.Shared.Models.DTO.LoginDTO;
using SenangMemberApp.Shared.Services.IService;
using System.Diagnostics;

namespace SenangMemberApp.Shared.ApiClient
{
    public class CompanyAC : BaseAC
    {
        public CompanyAC(HttpClient httpClient, ITokenService tokenService) : base(httpClient, tokenService)
        {

        }
        public async Task<ApiResponseRoot<List<CompanyResponseDTO>>> FetchCompanyList()
        {
            var response = await PostAsync<Object, ApiResponseRoot<List<CompanyResponseDTO>>>("api/PublicMember/GetCompanyList", null);

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, options);

            System.Diagnostics.Debug.WriteLine($"\n=== CREDIT API RESPONSE DUMP ===\n{jsonDebug}\n================================\n");

            return response;
        }
        public async Task<ApiResponseRoot<CompanyTokenResultDTO>> FetchCompanyToken(string id)
        {
            var response = await PostAsync<string, ApiResponseRoot<CompanyTokenResultDTO>>("api/PublicMember/LoginCompany", id);

            return response;
        }

        public async Task<ApiResponseRoot<List<CreditResponseDTO>>> FetchCompanyCreditBalanceDetails()
        {

            var response = await CompanyPostAsync<object, ApiResponseRoot<List<CreditResponseDTO>>>("api/publicMember/GetCreditBalanceDetails", null);
            if (response != null)
            {
                // 1. Convert the C# object into a readable, indented JSON string
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, options);

                System.Diagnostics.Debug.WriteLine($"\n=== CREDIT API RESPONSE DUMP ===\n{jsonDebug}\n================================\n");

                // 2. Print a quick summary so you don't have to scroll through the whole JSON
                var itemCount = response.result?.Count ?? 0;
                System.Diagnostics.Debug.WriteLine($"[SUMMARY] Status Code: {response.statusCode} | Total Credits Found: {itemCount}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\n[CREDIT API RESPONSE]: The response was completely NULL.\n");
            }
            return response;
        }

        public async Task<ApiResponseRoot<MemberBalanceDTO>> FetchCompanyMemberBalance()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<MemberBalanceDTO>>("api/publicMember/GetMemberBalanceSummary", null);
            return response;
        }

        public async Task<ApiResponseRoot<List<PackageResponseDTO>>> FetchCompanyPackageDetails()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<List<PackageResponseDTO>>>("api/PublicMember/GetPackageBalanceDetails", null);

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var debugResponse = System.Text.Json.JsonSerializer.Serialize(response, options);

            Debug.WriteLine(debugResponse);

            return response;
        }

        public async Task<ApiResponseRoot<List<BranchResponseDTO>>> FetchCompanyBranchDetails()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<List<BranchResponseDTO>>>("api/PublicMember/GetBranchesByLoginCompany", null);
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var debugResponse = System.Text.Json.JsonSerializer.Serialize(response, options);
            Debug.WriteLine(debugResponse);
            return response;
        }

        public async Task<ApiResponseRoot<List<BroadcastResponseDTO>>> FetchBroadcastWithinSixMonths()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<List<BroadcastResponseDTO>>>("api/PublicMember/GetLastSixMonthNews", null);

            return response;
        }

        public async Task<ApiResponseRoot<List<CatalogResponseDTO>>> FetchCompanyCategory()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<List<CatalogResponseDTO>>>("api/PublicMember/GetItemGroupsAsync", null);
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var debugResponse = System.Text.Json.JsonSerializer.Serialize(response, options);
            Debug.WriteLine(debugResponse);
            return response;
        }

        public async Task<ApiResponseRoot<List<ServiceResponseDTO>>> FetchCompanyServiceList(string itemGroupId)
        {
            var requestBody = new { id = itemGroupId };
            var response = await CompanyPostAsync<object, ApiResponseRoot<List<ServiceResponseDTO>>>("api/PublicMember/LoadByItemGroupIDAsync", requestBody);
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var debugResponse = System.Text.Json.JsonSerializer.Serialize(response, options);
            Debug.WriteLine(debugResponse);
            return response;
        }

        public async Task<RegisterApiResponseDTO?> RegisterAccount(string name, string email, string phoneNumber, string password)
        {
            var request = new RegisterRequestDTO
            {
                MemberName = name,
                Phone = phoneNumber,
                Email = email,
                Password = password
            };

            var response = await PostAnonymousAsync<RegisterRequestDTO, RegisterApiResponseDTO>("api/PublicMember/Register", request);

            return response;
        }
    }
}
