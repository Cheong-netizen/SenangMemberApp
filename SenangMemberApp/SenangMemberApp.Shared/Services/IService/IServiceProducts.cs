using SenangMemberApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Services.IService
{
    public interface IServiceProducts
    {
        List<CategoryModel> GetCategories();
        List<ServicesModel> GetServices();
        // Useful helper to filter services by category
        List<ServicesModel> GetServicesByCategory(int categoryId);
        List<ServicesModel> GetServicesByStoreId(int storeId);
        ServicesModel? GetServiceDetails(int serviceId);
    }
}
