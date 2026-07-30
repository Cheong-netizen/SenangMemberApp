using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.Repository
{
    public class OutletRepository : IOutletRepository
    {
        private readonly List<OutletModel> _outlets = new()
        {
            new OutletModel
            {
                Id = 1,
                ShopId = 1,
                Name = "Ampang Point Outlet",
                Address1 = "Lot G-05, Ground Floor",
                Address2 = "Ampang Point Shopping Centre",
                Address3 = "Jalan Mamanda 3",
                City = "Ampang",
                State = "Selangor",
                POSCode = "68000",
                StaffsId = new() { 1, 2 }
            },
            new OutletModel
            {
                Id = 2,
                ShopId = 1,
                Name = "Pavilion Elite Outlet",
                Address1 = "Level 4, Lot 4.22",
                Address2 = "Pavilion Kuala Lumpur",
                Address3 = "168, Jalan Bukit Bintang",
                City = "Kuala Lumpur",
                State = "WPKL",
                POSCode = "55100",
                StaffsId = new() { 3 }
            },
            new OutletModel
            {
                Id = 3,
                ShopId = 2,
                Name = "Bangsar Village Branch",
                Address1 = "Unit 1-2, First Floor",
                Address2 = "Bangsar Village II",
                Address3 = "Jalan Telawi 1",
                City = "Bangsar",
                State = "WPKL",
                POSCode = "59100",
                StaffsId = new() { 4 }
            }
        };

        public OutletModel? getOutletModelsByOutletId(int outletId)
        {
            return _outlets.SingleOrDefault(o => o.Id == outletId);
        }

        public List<OutletModel> getOutletModelsByShopId(int shopId)
        {
            return _outlets
                .Where(o => o.ShopId == shopId)
                .ToList();
        }
    }
}
