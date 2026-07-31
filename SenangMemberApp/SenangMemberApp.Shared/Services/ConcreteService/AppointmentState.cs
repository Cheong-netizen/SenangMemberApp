using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO.AppoinmentDTO;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class AppointmentState : IAppointmentState
    {
        private readonly AppointmentAC _appointmentAC;
        public AppointmentState(AppointmentAC appointmentAC)
        {
            _appointmentAC = appointmentAC;
        }
        public string selectedBookingShopName { get; private set; } = "";
        public string selectedBookingShopId { get; private set; } = "";
        public string selectedStaffId { get; private set; } = "";
        public HashSet<int> SelectedServiceIds { get; private set; } = new HashSet<int>();
        public TimeSpan TotalEstimateTime { get; private set; } = TimeSpan.Zero;
        public string selectedStaffName { get; private set; } = "";
        public string selectedOutletId { get; private set; } = "";
        public BranchResponseDTO selectedOutlet { get; private set; }
        public DateTime selectedTime { get; private set; }
        public string memo { get; set; }
        public void SetSelectedService(HashSet<int> selectedServiceIds, TimeSpan totalEstimateTime)
        {
            SelectedServiceIds = selectedServiceIds;
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
            string requestDateTime = selectedTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string requestEndTime = selectedTime.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            AppointmentCreateRequestDTO requestBody = new()
            {
                locationBranchId = selectedOutletId,
                appointmentDate = requestDateTime,
                endTime = requestEndTime,
                memo = memo
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
