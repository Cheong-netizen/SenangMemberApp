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

                // Assuming your API returns true/success status in response.result or similar.
                // You may need to adjust this conditional based on how your ApiResponseRoot is structured.
                if (response != null && response.result != null)
                {
                    // Success! Redirect to login page
                    NavManager.NavigateTo("/");
                }
                else
                {
                    // Registration failed, show error message from API if available
                    errorMessage = response?.message ?? "Registration failed. Please check your details and try again.";
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
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Phone Number is required.")]
            public string Phone { get; set; } = "";

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = "";
        }
    }
}