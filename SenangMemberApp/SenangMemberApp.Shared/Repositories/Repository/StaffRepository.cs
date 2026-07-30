using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private List<StaffModel> _staffs = new()
        {
            new StaffModel
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Tan",
                ShopId = 1,
                OutletId = 1,
                OccupiedTimeRange = new()
                {
                    new BlockedRange
                    {
                        Date = new DateTime(2026, 2, 5),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(11, 0, 0)
                    }
                }
            },
            new StaffModel
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Lim",
                ShopId = 1,
                OutletId = 2,
                OccupiedTimeRange = new()
                {
                    new BlockedRange
                    {
                        Date = new DateTime(2026, 2, 5),
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0)
                    }
                }
            },
            new StaffModel
            {
                Id = 3,
                FirstName = "Charlie",
                LastName = "Ng",
                ShopId = 2,
                OutletId = 2,
                OccupiedTimeRange = new()
                {
                    new BlockedRange
                    {
                        Date = new DateTime(2026, 2, 5),
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(15, 0, 0)
                    }
                }
            },
            new StaffModel
            {
                Id = 4,
                FirstName = "Diana",
                LastName = "Lee",
                ShopId = 2,
                OutletId = 3,
                OccupiedTimeRange = new()
                {
                    new BlockedRange
                    {
                        Date = new DateTime(2026, 2, 6),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(10, 30, 0)
                    }
                }
            }
        };

        public List<StaffModel> getAllStaffByOutletId(int outletId)
        {
            return _staffs.FindAll(s => s.OutletId == outletId);
        }

        public List<BlockedRange>? getStaffBlockedRange(int staffId)
        {
            return _staffs.Find(s => s.Id == staffId)?.OccupiedTimeRange;
        }
        public StaffModel? getStaffById(int staffId)
        {
            return _staffs.FirstOrDefault(s => s.Id == staffId);
        }
        public void addBlockedRangeToStaff(int staffId, BlockedRange blockedRange)
        {
            var staff = _staffs.FirstOrDefault(s => s.Id == staffId);
            if (staff != null)
                staff.OccupiedTimeRange.Add(blockedRange);
        }
    }
}
