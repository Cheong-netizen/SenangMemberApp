using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SenangMemberApp.Shared.Repositories.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private List<AppointmentModel> _appointments;

        public AppointmentRepository()
        {
            _appointments = new List<AppointmentModel>
            {
                // 1. Valid Appointment for Shop 1 (Main HQ) - Outlet 1 (Ampang)
                // Services: Women's Cut (101) + Underarm Wax (502)
                // Time: 60m + 15m = 75m
                new AppointmentModel
                {
                    id = 3,
                    ShopId = 1, // Matches ShopModel Id 1
                    OutletId = 1, // Matches OutletModel Id 1 (Ampang Point)
                    ServicesId = new HashSet<int> { 101, 502 },
                    TimeRequired = TimeSpan.FromMinutes(75),
                    AppointmentDateTime = DateTime.Now.AddDays(-1).Date.AddHours(11), // Tomorrow 11:00 AM
                    StaffId = 2
                },
                new AppointmentModel
                {
                    id = 4,
                    ShopId = 1, // Matches ShopModel Id 1
                    OutletId = 1, // Matches OutletModel Id 1 (Ampang Point)
                    ServicesId = new HashSet<int> { 101, 502 },
                    TimeRequired = TimeSpan.FromMinutes(75),
                    AppointmentDateTime = DateTime.Now.AddDays(1).Date.AddHours(11), // Tomorrow 11:00 AM
                    StaffId = 2
                },

                // 2. Valid Appointment for Shop 1 (Main HQ) - Outlet 2 (Pavilion)
                // Services: Anti-Aging Facial (302)
                // Time: 75m
                new AppointmentModel
                {
                    id = 5,
                    ShopId = 1,
                    OutletId = 2, // Matches OutletModel Id 2 (Pavilion Elite)
                    ServicesId = new HashSet<int> { 302 },
                    TimeRequired = TimeSpan.FromMinutes(75),
                    AppointmentDateTime = DateTime.Now.AddDays(2).Date.AddHours(14), // Day after tomorrow 2:00 PM
                    StaffId = 1
                },

                // 3. Valid Appointment for Shop 2 (Wellness) - Outlet 3 (Bangsar)
                // Services: Classic Manicure (201) + Gel Pedicure (202)
                // Time: 45m + 60m = 105m
                new AppointmentModel
                {
                    id = 6,
                    ShopId = 2, // Matches ShopModel Id 2
                    OutletId = 3, // Matches OutletModel Id 3 (Bangsar Village)
                    ServicesId = new HashSet<int> { 201, 203 },
                    TimeRequired = TimeSpan.FromMinutes(105),
                    AppointmentDateTime = DateTime.Now.Date.AddHours(15).AddMinutes(30), // Today 3:30 PM
                    StaffId = 3
                },

                // 4. Valid Appointment for Shop 2 (Wellness) - Outlet 3 (Bangsar)
                // Services: Bridal Glow Package (601)
                // Time: 180m
                new AppointmentModel
                {
                    id = 7,
                    ShopId = 2,
                    OutletId = 3,
                    ServicesId = new HashSet<int> { 601 },
                    TimeRequired = TimeSpan.FromMinutes(180),
                    AppointmentDateTime = DateTime.Now.AddDays(5).Date.AddHours(10), // In 5 days 10:00 AM
                    StaffId = 3
                },
                
                // 5. Short Service for Shop 2 - Outlet 3
                // Services: Head & Shoulder Relief (403)
                // Time: 30m
                new AppointmentModel
                {
                    id = 8,
                    ShopId = 2,
                    OutletId = 3,
                    ServicesId = new HashSet<int> { 403 },
                    TimeRequired = TimeSpan.FromMinutes(30),
                    AppointmentDateTime = DateTime.Now.AddDays(1).Date.AddHours(17), // Tomorrow 5:00 PM
                    StaffId = 4
                }
            };
        }

        public void addAppointment(AppointmentModel appointment)
        {
            if (appointment == null) return;

            // Simple logic to auto-increment ID for dummy data
            appointment.id = _appointments.Count > 0 ? _appointments.Max(a => a.id) + 1 : 1;

            _appointments.Add(appointment);
        }

        public List<AppointmentModel> getAppointment()
        {
            return _appointments;
        }

        public AppointmentModel? getAppointmentById(int id)
        {
            var result = _appointments.FirstOrDefault(a => a.id == id);
            return result;
        }
        public AppointmentModel? getLatestUpcomingAppointment()
        {
            var result = _appointments
                .Where(a => a.AppointmentDateTime >= DateTime.Today)
                .OrderBy(a => a.AppointmentDateTime)
                .FirstOrDefault();

            return result;
        }
        public AppointmentModel? getLatestUpcomingAppointment(int shopId = 0)
        {
            // 1. Get all future appointments
            var query = _appointments.Where(a => a.AppointmentDateTime >= DateTime.Today);

            // 2. IF shopId is NOT zero, filter by that ShopId
            if (shopId > 0)
            {
                query = query.Where(a => a.ShopId == shopId);
            }

            // 3. Order by date and take the first one
            return query
                .OrderBy(a => a.AppointmentDateTime)
                .FirstOrDefault();
        }
        public List<AppointmentModel> GetAppointmentsByShopIds(List<int> shopIds)
        {
            // If the list is null or empty, return ALL appointments (or return empty list based on your preference)
            if (shopIds == null || !shopIds.Any())
            {
                return _appointments;
            }

            // Filter appointments where the ShopId exists in the provided list
            return _appointments
                    .Where(a => shopIds.Contains(a.ShopId))
                    .ToList();
        }
    }
}