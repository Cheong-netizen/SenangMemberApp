using SenangMemberApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.IRepository
{
    public interface IStoreOutletRepository
    {
        List<ShopModel> GetShops();
        ShopModel? GetShopById(string id);
    }
}
