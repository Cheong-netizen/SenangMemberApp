using SenangMemberApp.Shared.Models;
using System.Collections.Generic;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IStoreService
    {
        List<ShopModel> GetAllShops();
        ShopModel? GetShopDetails(int shopId);
        List<OutletModel> GetOutletsByShopId(int shopId);
        List<StaffModel> GetStaffsByOutletId(int outletId);
        List<BlockedRange> GetStaffBlockedRange(int shopId, int outletId, int staffId);
        OutletModel? GetOutletDetails(int outletId);
        ServicesModel? GetServiceDetails(int serviceId);
        StaffModel? GetStaffDetails(int staffId);
    }
}