using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SenangMemberApp.Shared.ApiClient
{
    public class AppointmentAC: BaseAC
    {
        public AppointmentAC(HttpClient httpClient, ITokenService tokenService): base(httpClient, tokenService)
        {
            
        }

        public async Task<ApiResponseRoot<List<AppointmentResponseDTO>>> FetchCompanyAppointments(AppointmentRequestDTO requestPayload)
        {
            var response = await CompanyPostAsync<AppointmentRequestDTO, ApiResponseRoot<List<AppointmentResponseDTO>>>("/api/PublicMember/GetAppointments", requestPayload);
            Debug.WriteLine(JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            // 1. Convert the C# object into a readable, indented JSON string
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, options);

            System.Diagnostics.Debug.WriteLine($"\n=== CREDIT API RESPONSE DUMP ===\n{jsonDebug}\n================================\n");
            return response;
        }

        public async Task<ApiResponseRoot<AppointmentCreateResponseDTO>> RequestAppointmentCreation(AppointmentCreateRequestDTO requestPayload)
        {
            var requestOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var requestJson = System.Text.Json.JsonSerializer.Serialize(requestPayload, requestOptions);
            Console.WriteLine($"\n=== APPOINTMENT CREATION REQUEST DUMP ===\n{requestJson}\n=========================================\n");
            System.Diagnostics.Debug.WriteLine($"\n=== APPOINTMENT CREATION REQUEST DUMP ===\n{requestJson}\n=========================================\n");

            var response = await CompanyPostAsync<AppointmentCreateRequestDTO, ApiResponseRoot<AppointmentCreateResponseDTO>>("api/PublicMember/CreateAppointment", requestPayload);

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(response, options);

            Console.WriteLine($"\n=== CREDIT API RESPONSE DUMP APPOINTMENT CREATION ===\n{jsonDebug}\n================================\n");
            System.Diagnostics.Debug.WriteLine($"\n=== CREDIT API RESPONSE DUMP APPOINTMENT CREATION ===\n{jsonDebug}\n================================\n");
            return response;
        }

    }
}
