using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Services
{
    public class MobileShopStateLocalManagement : IShopStateLocalManagement
    {
        private const string ShopIdKey = "selected_shop_id";
        private const string ShopNameKey = "selected_shop_name";

        public async Task<(string id, string name)> GetShopSelection()
        {
            // Retrieve values from the device's secure keychain/keystore
            var idResult = await SecureStorage.GetAsync(ShopIdKey);
            var nameResult = await SecureStorage.GetAsync(ShopNameKey);

            // If we successfully found both pieces of data on the device
            if (!string.IsNullOrEmpty(idResult) && !string.IsNullOrEmpty(nameResult))
            {
                return (idResult, nameResult);
            }

            // Fallback: Return your default ID and your default Shop Name
            return ("0", "Select Shop");
        }

        public async Task SaveShopSelection(string id, string name)
        {
            // SecureStorage throws an exception if you pass a null value.
            // We use standard null checks to remove the key if it's null, or save it if it has data.

            if (id != null)
            {
                await SecureStorage.SetAsync(ShopIdKey, id);
            }
            else
            {
                SecureStorage.Remove(ShopIdKey);
            }

            if (name != null)
            {
                await SecureStorage.SetAsync(ShopNameKey, name);
            }
            else
            {
                SecureStorage.Remove(ShopNameKey);
            }
        }

        public Task ClearShopSelection()
        {
            SecureStorage.Remove(ShopIdKey);
            SecureStorage.Remove(ShopNameKey);
            return Task.CompletedTask;
        }
    }
}
