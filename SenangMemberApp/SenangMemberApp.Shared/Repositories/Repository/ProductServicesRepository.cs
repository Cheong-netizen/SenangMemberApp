using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Repositories.IRepository;

namespace SenangMemberApp.Shared.Repositories.Repository
{
    public class ProductServicesRepository : IProductServicesRepository
    {
        private List<CategoryModel> _categories;
        private List<ServicesModel> _services;

        public ProductServicesRepository()
        {
            // Initialize Categories
            _categories = new List<CategoryModel>
            {
                new() { Id = 1, Name = "Hair" },
                new() { Id = 2, Name = "Nails" },
                new() { Id = 3, Name = "Face" },
                new() { Id = 4, Name = "Massage" },
                new() { Id = 5, Name = "Waxing" },
                new() { Id = 6, Name = "Packages" }
            };

            // Initialize Services
            _services = new List<ServicesModel>
            {
                // --- 1. Hair Services ---
                new() {
                    Id = 101, CategoryId = 1, CategoryName = "Hair", Name = "Women's Cut & Blow Dry", Price = 65, ShopId = 1,
                    Description = "Includes consultation, shampoo, cut, and professional styling.",
                    EstimatedDuration = TimeSpan.FromMinutes(60) // Added
                },
                new() {
                    Id = 102, CategoryId = 1, CategoryName = "Hair", Name = "Men's Standard Cut", Price = 45, ShopId = 1,
                    Description = "Precision clipper and scissor cut with wash.",
                    EstimatedDuration = TimeSpan.FromMinutes(30) // Added
                },
                new() {
                    Id = 103, CategoryId = 1, CategoryName = "Hair", Name = "Root Touch-Up", Price = 120, ShopId = 1,
                    Description = "Color application for root regrowth (up to 2 inches).",
                    EstimatedDuration = TimeSpan.FromMinutes(90) // Added
                },
                new() {
                    Id = 104, CategoryId = 1, CategoryName = "Hair", Name = "Keratin Treatment", Price = 350, ShopId = 1,
                    Description = "Smoothing treatment to reduce frizz and increase shine.",
                    EstimatedDuration = TimeSpan.FromMinutes(150) // Added
                },

                // --- 2. Nail Services ---
                new() {
                    Id = 201, CategoryId = 2, CategoryName = "Nails", Name = "Classic Manicure", Price = 40, ShopId = 2,
                    Description = "Nail shaping, cuticle care, hand massage, and polish.",
                    EstimatedDuration = TimeSpan.FromMinutes(45)
                },
                new() {
                    Id = 202, CategoryId = 2, CategoryName = "Nails", Name = "Gel Pedicure", Price = 85, ShopId = 2,
                    Description = "Long-lasting gel polish with full foot spa and scrub.",
                    EstimatedDuration = TimeSpan.FromMinutes(60)
                },
                new() {
                    Id = 203, CategoryId = 2, CategoryName = "Nails", Name = "Nail Art (Per Finger)", Price = 5, ShopId = 2,
                    Description = "Custom designs added to your manicure service.",
                    EstimatedDuration = TimeSpan.FromMinutes(15)
                },

                // --- 3. Face Services ---
                new() {
                    Id = 301, CategoryId = 3, CategoryName = "Face", Name = "Signature Hydrating Facial", Price = 180, ShopId = 1,
                    Description = "60-minute deep hydration treatment for glowing skin.",
                    EstimatedDuration = TimeSpan.FromMinutes(60)
                },
                new() {
                    Id = 302, CategoryId = 3, CategoryName = "Face", Name = "Anti-Aging Collagen Facial", Price = 250, ShopId = 1,
                    Description = "Firming and lifting treatment targeting fine lines.",
                    EstimatedDuration = TimeSpan.FromMinutes(75)
                },
                new() {
                    Id = 303, CategoryId = 3, CategoryName = "Face", Name = "Classic Eyelash Extensions", Price = 150, ShopId = 1,
                    Description = "Natural looking lash extensions (full set).",
                    EstimatedDuration = TimeSpan.FromMinutes(90)
                },

                // --- 4. Massage Services ---
                new() {
                    Id = 401, CategoryId = 4, CategoryName = "Massage", Name = "Aromatherapy Massage (60m)", Price = 130, ShopId = 2,
                    Description = "Relaxing full body massage using essential oils.",
                    EstimatedDuration = TimeSpan.FromMinutes(60)
                },
                new() {
                    Id = 402, CategoryId = 4, CategoryName = "Massage", Name = "Deep Tissue Massage (90m)", Price = 200, ShopId = 2,
                    Description = "Intense pressure to relieve chronic muscle tension.",
                    EstimatedDuration = TimeSpan.FromMinutes(90)
                },
                new() {
                    Id = 403, CategoryId = 4, CategoryName = "Massage", Name = "Head & Shoulder Relief", Price = 60, ShopId = 2,
                    Description = "30-minute focus on upper body stress points.",
                    EstimatedDuration = TimeSpan.FromMinutes(30)
                },

                // --- 5. Waxing Services ---
                new() {
                    Id = 501, CategoryId = 5, CategoryName = "Waxing", Name = "Full Leg Waxing", Price = 90, ShopId = 1,
                    Description = "Smooth skin from thigh to ankle.",
                    EstimatedDuration = TimeSpan.FromMinutes(45)
                },
                new() {
                    Id = 502, CategoryId = 5, CategoryName = "Waxing", Name = "Underarm Waxing", Price = 35, ShopId = 1,
                    Description = "Gentle hair removal for sensitive skin.",
                    EstimatedDuration = TimeSpan.FromMinutes(15)
                },
                new() {
                    Id = 503, CategoryId = 5, CategoryName = "Waxing", Name = "Eyebrow Threading", Price = 15, ShopId = 1,
                    Description = "Precise shaping using thread technique.",
                    EstimatedDuration = TimeSpan.FromMinutes(15)
                },

                // --- 6. Packages ---
                new() {
                    Id = 601, CategoryId = 6, CategoryName = "Packages", Name = "Bridal Glow Package", Price = 550, ShopId = 2,
                    Description = "Includes Premium Facial, Gel Mani-Pedi, and Body Scrub.",
                    EstimatedDuration = TimeSpan.FromMinutes(180)
                },
            };
        }

        public List<CategoryModel> GetCategories()
        {
            return _categories;
        }

        public List<ServicesModel> GetServices()
        {
            return _services;
        }

        public List<ServicesModel> GetServicesByCategory(int categoryId)
        {
            return _services.Where(s => s.CategoryId == categoryId).ToList();
        }
        public List<ServicesModel> GetServicesByStore(int shopId)
        {
            return _services
                .Where(s => s.ShopId == shopId)
                .ToList();
        }
        public ServicesModel? GetServiceById(int serviceId)
        {
            return _services.FirstOrDefault(s => s.Id == serviceId);
        }
    }
}