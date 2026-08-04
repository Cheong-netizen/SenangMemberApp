using SenangMemberApp.Shared.ApiClient;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Register
    {
        [Inject]
        private CompanyAC companyAC { get; set; } = default!;

        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        private RegisterModel registerRequest = new();
        private bool isLoading = false;
        private string errorMessage = "";

        private async Task HandleRegister()
        {
            isLoading = true;
            errorMessage = "";

            try
            {
                var response = await companyAC.RegisterAccount(
                    registerRequest.Name,
                    registerRequest.Email,
                    registerRequest.Phone,
                    registerRequest.Password
                );

                if (response != null && !response.IsError && (response.Result != null || response.Status == 200 || response.StatusCode == 200))
                {
                    // Success! Redirect to login page
                    NavManager.NavigateTo("/");
                }
                else
                {
                    // Registration failed, show error message from API if available
                    if (response != null && !string.IsNullOrWhiteSpace(response.Title))
                    {
                        errorMessage = response.Title;
                    }
                    else if (response != null && !string.IsNullOrWhiteSpace(response.Message))
                    {
                        errorMessage = response.Message;
                    }
                    else if (response != null && !string.IsNullOrWhiteSpace(response.Details))
                    {
                        errorMessage = response.Details;
                    }
                    else
                    {
                        errorMessage = "Registration failed. Please check your details and try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "An unexpected error occurred. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        // Internal class specifically for form validation on this page
        public class RegisterModel
        {
            [Required(ErrorMessage = "Full Name is required.")]
            public string Name { get; set; } = "";

            [Required(ErrorMessage = "Email Address is required.")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Phone Number is required.")]
            public string Phone { get; set; } = "";

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = "";
        }
    }
}