using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Models.DTO;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SenangMemberApp.Shared.Pages.CatalogPages
{
    public partial class Product
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private CompanyAC companyAC { get; set; } = default!;
        [Parameter]
        public string CategoryId { get; set; }
        List<ServiceResponseDTO> services = new();
        protected override async Task OnInitializedAsync()
        {
            var response = await companyAC.FetchCompanyServiceList(CategoryId);
            services = response.result ?? new();
        }
        private void navBack()
        {
            NavigationManager.NavigateTo("/catalog");
        }

    }
}
