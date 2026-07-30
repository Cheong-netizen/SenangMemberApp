//using Microsoft.AspNetCore.Components;
//using SenangMemberApp.Shared.Models;
//using SenangMemberApp.Shared.Services.IService; // Import the Interface namespace
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.AspNetCore.Components.Web;
//using SenangMemberApp.Shared.Services.ConcreteService;

//namespace SenangMemberApp.Shared.Pages.AppointmentPages
//{
//    public partial class AppointmentSelectServices
//    {
//        [Inject]
//        public required IServiceProducts ProductService { get; set; } 

//        [Inject]
//        public required NavigationManager NavManager { get; set; }
//        [Inject]
//        private IAppointmentState appointmentState { get; set; } = default!;
//        private string servicesSearchText = string.Empty;

//        private int _selectedCategoryId = 0; // 0 represents "All"

//        private List<CategoryModel> Categories = new();
//        private List<ServicesModel> AllItems = new();
//        private DateTime timeClick { get; set; }
//        private DateTime timeRelease { get; set; }
//        private bool servicesModalIsOpen = false;
//        private HashSet<int> SelectedServiceIds = new();
//        private CancellationTokenSource? pressCts;
//        private bool isLongPress = false;
//        private double startX;
//        private double startY;
//        private int MoveThreshold = 10;
//        private int modalServiceIdToDisplay = 0;
//        private ServicesModel serviceToShowInModal = new();
//        private IEnumerable<ServicesModel> FilteredItems
//        {
//            get
//            {
//                IEnumerable<ServicesModel> query = AllItems.AsEnumerable();

//                if(_selectedCategoryId != 0)
//                {
//                    query = query.Where(q => q.CategoryId == _selectedCategoryId);
//                }

//                if (!string.IsNullOrEmpty(servicesSearchText))
//                {
//                    query = query.Where(s => s.Name.Contains(servicesSearchText, StringComparison.OrdinalIgnoreCase));
//                }

//                return query;
//            }
//        }

//        protected override void OnInitialized()
//        {
//            Categories = ProductService.GetCategories();
//            AllItems = ProductService.GetServicesByStoreId(appointmentState.CurrentAppointment.ShopId);
//        }

//        private void FilterByCategory(int categoryId)
//        {
//            _selectedCategoryId = categoryId;
//        }

//        private void navSelectStaff()
//        {
//            if (SelectedServiceIds.Count < 1)
//                return;

//            TimeSpan totalEstimateTime = getTotalEstimatedTime(SelectedServiceIds);
//            appointmentState.SetService(SelectedServiceIds, totalEstimateTime);
//            NavManager.NavigateTo("/AppointmentSelectStaff");
//        }
//        private TimeSpan getTotalEstimatedTime(HashSet<int> selectedServiceIds)
//        {
//            TimeSpan totalTime = AllItems
//                .Where(s => selectedServiceIds.Contains(s.Id))
//                .Aggregate(TimeSpan.Zero, (total, next) => total + next.EstimatedDuration);

//            return totalTime;
//        }
//        private async Task itemClicked(int serviceId, PointerEventArgs e)
//        {
//            //timeClick = DateTime.Now;
//            startX = e.ClientX;
//            startY = e.ClientY;
//            isLongPress = false;
//            pressCts = new CancellationTokenSource();
//            try
//            {
//                await Task.Delay(400, pressCts.Token);

//                isLongPress = true;
//                servicesModalIsOpen = true;
//                modalServiceIdToDisplay = serviceId;
//                serviceToShowInModal = AllItems.First(i => i.Id == serviceId);
//            }
//            catch (TaskCanceledException)
//            {

//            }
//        }

//        private void itemReleased(int serviceId)
//        {
//            //timeRelease = DateTime.Now;
//            //TimeSpan duration = timeRelease - timeClick;
//            //if (duration.TotalMilliseconds > 400) 
//            //{
//            //    servicesModalIsOpen = true;
//            //}
//            //else
//            //{
//            //    SelectedServiceIds.Add(serviceId);
//            //}
//            pressCts?.Cancel();

//            if (isLongPress)
//                return;

//            if (!SelectedServiceIds.Add(serviceId))
//            {
//                SelectedServiceIds.Remove(serviceId);
//            }

//            else
//            {
//                SelectedServiceIds.Add(serviceId);
//            }
//        }
//        private void onPointerMove(PointerEventArgs e)
//        {
//            double dx = Math.Abs(e.ClientX - startX);
//            double dy = Math.Abs(e.ClientY - startY);

//            if (dx > MoveThreshold || dy > MoveThreshold)
//            {
//                pressCts?.Cancel();
//            }
//        }
//        private void closeServiceModal()
//        {
//            servicesModalIsOpen = false;
//        }
//        private void navBack()
//        {
//            NavManager.NavigateTo("/AppointmentSelectOutlet");
//        }
//    }
//}