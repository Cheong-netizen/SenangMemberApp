using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.PurchaseHistoryDTO;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SenangMemberApp.Shared.ApiClient
{
    public class PurchaseHistoryAC : BaseAC
    {
        public PurchaseHistoryAC(HttpClient httpClient, ITokenService tokenService) : base(httpClient, tokenService)
        {

        }

        public async Task<ApiResponseRoot<List<PurchaseHistoryMonthlyResponseDTO>>> GetCustomerServiceRecordByMonthAsync()
        {
            var response = await CompanyPostAsync<object, ApiResponseRoot<List<PurchaseHistoryMonthlyResponseDTO>>>("api/PublicMember/GetCustomerServiceRecordYearMonth", null);
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, options);

            return response;
        }

        public async Task<ApiResponseRoot<Dictionary<string, List<ServiceRecordResponseDTO>>>> GetCustomerServiceRecordByMonthAsync(string id)
        {
            // 1. Wrap the string in an object so it serializes correctly
            var requestBody = new { id = id };

            // 2. Pass the object, and ensure the first generic type is 'object' (or whatever your wrapper expects for anonymous types)
            var response = await CompanyPostAsync<object, ApiResponseRoot<Dictionary<string, List<ServiceRecordResponseDTO>>>>("api/PublicMember/GetCustomerServiceRecordByMonth", requestBody);

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, options);

            Debug.WriteLine(jsonDebug);
            return response;
        }
    }
}
