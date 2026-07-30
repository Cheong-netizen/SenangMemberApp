using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Models.DTO.CompanyDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IAppointmentState
    {
        public string selectedStaffId { get; }
        public HashSet<int> SelectedServiceIds { get; } 
        public TimeSpan TotalEstimateTime { get; } 
        public string selectedStaffName { get; }
        public string selectedBookingShopName { get; }
        public string selectedBookingShopId { get; }
        public string selectedOutletId { get; }
        public BranchResponseDTO selectedOutlet { get; }
        public string memo { get; }
        public DateTime selectedTime { get; }
        public void SetBookingShop(string id, string shopName);
        public void SetOutlet(string outletId);
        public void SetTime(DateTime time);
        public Task<bool> ConfirmAppointment();
        public void SetOutlet(BranchResponseDTO outlet);
        public void SetSelectedService(HashSet<int> selectedServiceIds, TimeSpan totalEstimateTime);
        public void SetSelectedStaff(string staffId, string staffName);
    }
}
