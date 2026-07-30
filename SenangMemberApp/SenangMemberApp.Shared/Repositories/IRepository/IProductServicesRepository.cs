using SenangMemberApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Repositories.IRepository
{
    public interface IProductServicesRepository
    {
        List<CategoryModel> GetCategories();
        List<ServicesModel> GetServices();
        List<ServicesModel> GetServicesByCategory(int categoryId);
        List<ServicesModel> GetServicesByStore(int shopId);
        ServicesModel? GetServiceById(int serviceId);
    }
}
