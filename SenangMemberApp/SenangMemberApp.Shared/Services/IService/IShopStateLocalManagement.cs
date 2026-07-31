using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IShopStateLocalManagement
    {
        Task SaveShopSelection(string id, string name);
        Task<(string id, string name)> GetShopSelection();
        Task ClearShopSelection();
    }
}
