using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class AppointmentState : IAppointmentState
    {
        private readonly AppointmentAC _appointmentAC;
        private readonly IUserProfileService _userProfileService;

        public AppointmentState(AppointmentAC appointmentAC, IUserProfileService userProfileService)
        {
            _appointmentAC = appointmentAC;
            _userProfileService = userProfileService;
        }
        public string selectedBookingShopName { get; private set; } = "";
        public string selectedBookingShopId { get; private set; } = "";
        public string selectedStaffId { get; private set; } = "";
        public HashSet<int> SelectedServiceIds { get; private set; } = new HashSet<int>();
        public HashSet<string> SelectedServiceCodes { get; private set; } = new HashSet<string>();
        public List<ServiceResponseDTO> SelectedServices { get; private set; } = new List<ServiceResponseDTO>();
        public TimeSpan TotalEstimateTime { get; private set; } = TimeSpan.Zero;
        public string selectedStaffName { get; private set; } = "";
        public string selectedOutletId { get; private set; } = "";
        public BranchResponseDTO selectedOutlet { get; private set; }
        public DateTime selectedTime { get; private set; }
        public string memo { get; set; } = "";
        public void SetSelectedService(HashSet<int> selectedServiceIds, TimeSpan totalEstimateTime)
        {
            SelectedServiceIds = selectedServiceIds;
            TotalEstimateTime = totalEstimateTime;
        }
        public void SetSelectedServices(List<ServiceResponseDTO> selectedServices, TimeSpan totalEstimateTime)
        {
            SelectedServices = selectedServices ?? new List<ServiceResponseDTO>();
            SelectedServiceCodes = new HashSet<string>(SelectedServices.Select(s => s.masterAccountID ?? s.displayCode ?? "").Where(c => !string.IsNullOrEmpty(c)));
            TotalEstimateTime = totalEstimateTime;
        }
        public void SetSelectedStaff(string staffId, string staffName)
        {
            selectedStaffId = staffId;
            selectedStaffName = staffName;
        }

        public void SetBookingShop(string id, string shopName)
        {
            selectedBookingShopId = id;
            selectedBookingShopName = shopName;
        }

        public void SetOutlet(string outletId)
        {
            selectedOutletId = outletId;
        }

        public void SetOutlet(BranchResponseDTO outlet)
        {
            selectedOutlet = outlet;
        }

        public void SetTime(DateTime time)
        {
            selectedTime = time;
        }
        public async Task<bool> ConfirmAppointment()
        {
            var date = selectedTime;
            var durationMinutes = TotalEstimateTime.TotalMinutes > 0 ? (int)TotalEstimateTime.TotalMinutes : 120;
            var branchId = selectedOutlet?.branchID ?? selectedOutletId ?? "";
            var employeeId = selectedStaffId;
            var serviceId = string.Join(",", SelectedServices.Select(s => s.masterAccountID ?? s.displayCode).Where(c => !string.IsNullOrEmpty(c)));
            if (string.IsNullOrEmpty(serviceId))
            {
                serviceId = SelectedServiceCodes.FirstOrDefault() ?? "";
            }

            var profileRes = await _userProfileService.GetUserProfile();
            var customerId = profileRes?.result?.Phone ?? "";
            var customerName = profileRes?.result?.AccountName ?? "";

            AppointmentCreateRequestDTO requestBody = new()
            {
                appointmentID = (string?)null,
                // These names intentionally match EBI.DM.AppointmentDM in the
                // published Swagger contract. appointmentDate/locationBranchID
                // are not AppointmentDM properties and EBI can silently ignore them.
                startTime = date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                branchID = branchId,
                employeeID = string.IsNullOrWhiteSpace(employeeId) || employeeId.Equals("Any", StringComparison.OrdinalIgnoreCase) ? "Online" : employeeId,
                customerID = customerId,
                customerName = customerName,
                memo = memo,
                endTime = date.AddMinutes(durationMinutes).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                inventoryIDs = serviceId,
                saveAction = "Added",
                isDirty = true
            };

            var response = await _appointmentAC.RequestAppointmentCreation(requestBody);
            if (response != null && response.statusCode == 200)
            {
                return true;
            }
            return false;
        }
    }
}
