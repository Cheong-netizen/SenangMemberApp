using SenangMemberApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.IRepository
{
    public interface IStaffRepository
    {
        List<StaffModel> getAllStaffByOutletId(int outletId);
        List<BlockedRange>? getStaffBlockedRange(int staffId);
        StaffModel? getStaffById(int staffId);
        void addBlockedRangeToStaff(int staffId, BlockedRange blockedRange);
    }
}
