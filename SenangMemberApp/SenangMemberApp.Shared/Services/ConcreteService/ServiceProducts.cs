using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Services.IService;
using System.Collections.Generic;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class ServiceProducts : IServiceProducts
    {
        private readonly IProductServicesRepository _repository;

        // Constructor Injection
        public ServiceProducts(IProductServicesRepository repository)
        {
            _repository = repository;
        }

        public List<CategoryModel> GetCategories()
        {
            return _repository.GetCategories();
        }

        public List<ServicesModel> GetServices()
        {
            return _repository.GetServices();
        }

        public List<ServicesModel> GetServicesByCategory(int categoryId)
        {
            return _repository.GetServicesByCategory(categoryId);
        }
        public List<ServicesModel> GetServicesByStoreId(int storeId)
        {
            return _repository.GetServicesByStore(storeId);
        }
        public ServicesModel? GetServiceDetails(int serviceId)
        {
            return _repository.GetServiceById(serviceId);
        }

    }
}