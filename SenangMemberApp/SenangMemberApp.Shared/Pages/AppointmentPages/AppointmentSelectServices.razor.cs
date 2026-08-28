using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSelectServices
    {
        [Inject]
        public required NavigationManager NavManager { get; set; }

        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;

        [Inject]
        private CompanyAC companyAC { get; set; } = default!;

        private string servicesSearchText = string.Empty;
        private string _selectedCategoryId = string.Empty; // Empty string represents "All"

        private List<CatalogResponseDTO> Categories = new();
        private List<ServiceResponseDTO> AllItems = new();
        private Dictionary<string, List<ServiceResponseDTO>> _servicesByCategory = new();
        private HashSet<string> SelectedServiceCodes = new();
        private List<ServiceResponseDTO> SelectedServices = new();

        private bool isLoading = true;
        private bool servicesModalIsOpen = false;
        private ServiceResponseDTO serviceToShowInModal = new();

        private CancellationTokenSource? pressCts;
        private bool isLongPress = false;
        private double startX;
        private double startY;
        private int MoveThreshold = 10;

        private IEnumerable<ServiceResponseDTO> FilteredItems
        {
            get
            {
                IEnumerable<ServiceResponseDTO> query;

                if (string.IsNullOrEmpty(_selectedCategoryId))
                {
                    query = AllItems.AsEnumerable();
                }
                else if (_servicesByCategory.TryGetValue(_selectedCategoryId, out var catItems))
                {
                    query = catItems.AsEnumerable();
                }
                else
                {
                    query = AllItems.Where(q => q.itemGroupID == _selectedCategoryId);
                }

                // Filter by inventoryTypeID == 3 (Services)
                query = query.Where(q => q.inventoryTypeID == 3);

                if (!string.IsNullOrWhiteSpace(servicesSearchText))
                {
                    query = query.Where(s =>
                        (!string.IsNullOrEmpty(s.salesDescription) && s.salesDescription.Contains(servicesSearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(s.itemGroupName) && s.itemGroupName.Contains(servicesSearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(s.remarks) && s.remarks.Contains(servicesSearchText, StringComparison.OrdinalIgnoreCase))
                    );
                }

                return query;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            // Restore previous state if available
            if (appointmentState.SelectedServiceCodes != null && appointmentState.SelectedServiceCodes.Any())
            {
                var firstCode = appointmentState.SelectedServiceCodes.First();
                SelectedServiceCodes = new HashSet<string> { firstCode };
                var firstService = appointmentState.SelectedServices?.FirstOrDefault();
                SelectedServices = firstService != null ? new List<ServiceResponseDTO> { firstService } : new();
            }

            try
            {
                // 1. Fetch Item Groups / Categories
                var categoryResponse = await companyAC.FetchCompanyCategory();
                Categories = categoryResponse?.result ?? new List<CatalogResponseDTO>();

                // 2. Fetch Services for each category using LoadByItemGroupIDAsync
                if (Categories.Any())
                {
                    var fetchTasks = Categories.Select(async cat =>
                    {
                        var svcResponse = await companyAC.FetchCompanyServiceList(cat.id);
                        var services = (svcResponse?.result ?? new List<ServiceResponseDTO>())
                            .Where(s => s.inventoryTypeID == 3)
                            .ToList();
                        return (cat.id, services);
                    });

                    var results = await Task.WhenAll(fetchTasks);
                    _servicesByCategory = results.ToDictionary(r => r.id, r => r.services);

                    // Aggregate all unique services
                    var combinedList = new List<ServiceResponseDTO>();
                    var seenCodes = new HashSet<string>();

                    foreach (var groupServices in _servicesByCategory.Values)
                    {
                        foreach (var service in groupServices)
                        {
                            var code = GetServiceKey(service);
                            if (!string.IsNullOrEmpty(code))
                            {
                                if (seenCodes.Add(code))
                                {
                                    combinedList.Add(service);
                                }
                            }
                            else
                            {
                                combinedList.Add(service);
                            }
                        }
                    }

                    AllItems = combinedList;
                }
                else
                {
                    // Fallback to fetch with empty id if no categories returned
                    var svcResponse = await companyAC.FetchCompanyServiceList("");
                    AllItems = (svcResponse?.result ?? new List<ServiceResponseDTO>())
                        .Where(s => s.inventoryTypeID == 3)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppointmentSelectServices Error] {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private string GetServiceKey(ServiceResponseDTO service)
        {
            return !string.IsNullOrEmpty(service.masterAccountID)
                ? service.masterAccountID
                : (!string.IsNullOrEmpty(service.displayCode) ? service.displayCode : service.salesDescription ?? "");
        }

        private void FilterByCategory(string categoryId)
        {
            _selectedCategoryId = categoryId;
        }

        private void navSelectStaff()
        {
            if (SelectedServiceCodes.Count != 1)
                return;

            var service = SelectedServices.FirstOrDefault();
            int minutes = (service?.serviceMinutes != null && service.serviceMinutes > 0)
                ? service.serviceMinutes.Value
                : 120; // Default to 2 hours if 0 or null

            TimeSpan totalEstimateTime = TimeSpan.FromMinutes(minutes);
            appointmentState.SetSelectedServices(SelectedServices, totalEstimateTime);
            NavManager.NavigateTo("/AppointmentSelectStaff");
        }

        private async Task itemClicked(ServiceResponseDTO service, PointerEventArgs e)
        {
            startX = e.ClientX;
            startY = e.ClientY;
            isLongPress = false;
            pressCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(400, pressCts.Token);

                isLongPress = true;
                serviceToShowInModal = service;
                servicesModalIsOpen = true;
                StateHasChanged();
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void itemReleased(ServiceResponseDTO service)
        {
            pressCts?.Cancel();

            if (isLongPress)
                return;

            var key = GetServiceKey(service);
            if (string.IsNullOrEmpty(key)) return;

            if (SelectedServiceCodes.Contains(key))
            {
                SelectedServiceCodes.Remove(key);
                SelectedServices.RemoveAll(s => GetServiceKey(s) == key);
            }
            else
            {
                SelectedServiceCodes.Clear();
                SelectedServiceCodes.Add(key);
                SelectedServices.Clear();
                SelectedServices.Add(service);
            }
        }

        private void onPointerMove(PointerEventArgs e)
        {
            double dx = Math.Abs(e.ClientX - startX);
            double dy = Math.Abs(e.ClientY - startY);

            if (dx > MoveThreshold || dy > MoveThreshold)
            {
                pressCts?.Cancel();
            }
        }

        private void closeServiceModal()
        {
            servicesModalIsOpen = false;
        }

        private void navBack()
        {
            NavManager.NavigateTo("/AppointmentSelectOutlet");
        }
    }
}