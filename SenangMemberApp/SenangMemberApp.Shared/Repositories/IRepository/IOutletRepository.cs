using SenangMemberApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.IRepository
{
    public interface IOutletRepository
    {
        List<OutletModel> getOutletModelsByShopId(int shopId);
        OutletModel? getOutletModelsByOutletId(int outletId);
    }
}
