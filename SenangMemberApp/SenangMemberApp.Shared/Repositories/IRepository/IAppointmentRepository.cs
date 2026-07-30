using SenangMemberApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.IRepository
{
    public interface IAppointmentRepository
    {
        List<AppointmentModel> getAppointment();
        void addAppointment(AppointmentModel appointment);
        AppointmentModel? getAppointmentById(int id);
        AppointmentModel? getLatestUpcomingAppointment();
        AppointmentModel? getLatestUpcomingAppointment(int shopId = 0);
        List<AppointmentModel> GetAppointmentsByShopIds(List<int> shopIds);
    }
}
