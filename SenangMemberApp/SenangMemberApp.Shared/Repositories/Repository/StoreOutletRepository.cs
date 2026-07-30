using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;

namespace SenangMemberApp.Shared.Repositories.Repository
{
    public class StoreOutletRepository : IStoreOutletRepository
    {
        private readonly List<ShopModel> _shops = new()
        {
            new ShopModel
            {
                Id = "1",
                Name = "EBI Beauty - Main HQ"
            },
            new ShopModel
            {
                Id = "2",
                Name = "EBI Wellness - City Center"
            }
        };

        public List<ShopModel> GetShops()
        {
            return _shops;
        }

        public ShopModel? GetShopById(string id)
        {
            return _shops.FirstOrDefault(s => s.Id == id);
        }
    }
}