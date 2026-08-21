using SenangMemberApp.Shared.ApiClient;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class Register : IDisposable
    {
        [Inject]
        private HttpClient HttpClient { get; set; } = default!;

        [Inject]
        private CompanyAC companyAC { get; set; } = default!;

        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        private RegisterModel registerRequest = new();
        private bool isLoading = false;
        private bool isSuccess = false;
        private bool isCodeSent = false;
        private bool isPhonePadOpen = false;

        private void OpenPhonePad()
        {
            isPhonePadOpen = true;
        }

        private void OnPhoneSelected(string phone)
        {
            registerRequest.Phone = phone;
            StateHasChanged();
        }
        private string inputOtpCode = "";
        private string generatedOtpCode = "";
        private string errorMessage = "";
        private string successMessage = "";
        private int resendCountdown = 0;
        private bool isCooldownActive => resendCountdown > 0;
        private System.Threading.CancellationTokenSource? _countdownCts;

        private const string GreenApiUrl = "https://7105.api.greenapi.com/waInstance7105472363/sendMessage/5a7db8f511c24d7abbefd0e2cec36ba50c07b35615fe44c19d";

        private async Task StartResendCountdown(int seconds = 10)
        {
            _countdownCts?.Cancel();
            _countdownCts = new System.Threading.CancellationTokenSource();
            var token = _countdownCts.Token;

            resendCountdown = seconds;
            StateHasChanged();

            try
            {
                while (resendCountdown > 0 && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    resendCountdown--;
                    StateHasChanged();
                }
            }
            catch (TaskCanceledException)
            {
                // Ignored when cancelled or disposed
            }
        }

        private async Task HandleSendVerificationCode()
        {
            errorMessage = "";
            successMessage = "";

            if (string.IsNullOrWhiteSpace(registerRequest.Phone))
            {
                errorMessage = "Please enter a valid phone number.";
                return;
            }

            isLoading = true;

            try
            {
                // Generate 6-digit random code
                Random random = new Random();
                generatedOtpCode = random.Next(100000, 999999).ToString();

                // Format chatId: e.g. 60183208832@c.us
                string cleanedPhone = registerRequest.Phone.Replace("+", "").Replace(" ", "").Replace("-", "").Trim();
                if (!cleanedPhone.EndsWith("@c.us"))
                {
                    cleanedPhone = $"{cleanedPhone}@c.us";
                }

                var payload = new
                {
                    chatId = cleanedPhone,
                    message = $"Your SenangMember registration verification code is: {generatedOtpCode}",
                    typingTime = 1000
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(GreenApiUrl, content);
                var responseText = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[GreenAPI WhatsApp] Status: {response.StatusCode}, Response: {responseText}");

                if (response.IsSuccessStatusCode)
                {
                    isCodeSent = true;
                    successMessage = $"A 6-digit verification code has been sent to your WhatsApp ({registerRequest.Phone}).";
                    _ = StartResendCountdown(10);
                }
                else
                {
                    errorMessage = $"Failed to send WhatsApp message: {responseText}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GreenAPI Exception] {ex.Message}");
                errorMessage = "Failed to send WhatsApp verification code. Please try again.";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task HandleResendCode()
        {
            if (isLoading || isCooldownActive)
            {
                return;
            }

            await HandleSendVerificationCode();
        }

        private async Task HandleVerifyAndRegister()
        {
            errorMessage = "";
            successMessage = "";

            if (string.IsNullOrWhiteSpace(inputOtpCode))
            {
                errorMessage = "Please enter the 6-digit verification code.";
                return;
            }

            if (inputOtpCode.Trim() != generatedOtpCode)
            {
                errorMessage = "Incorrect verification code. Please check and try again.";
                return;
            }

            isLoading = true;

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
                    // Success! Show confirmation and redirect to login page
                    isSuccess = true;
                    successMessage = "Registration successful! Redirecting to login page...";
                    StateHasChanged();

                    await Task.Delay(1500);
                    NavManager.NavigateTo("/", replace: true);
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
                StateHasChanged();
            }
        }

        private void ChangeDetails()
        {
            isCodeSent = false;
            inputOtpCode = "";
            errorMessage = "";
            successMessage = "";
            StateHasChanged();
        }

        public void Dispose()
        {
            _countdownCts?.Cancel();
            _countdownCts?.Dispose();
        }

        // Internal class specifically for form validation on this page
        public class RegisterModel
        {
            [Required(ErrorMessage = "Full Name is required.")]
            public string Name { get; set; } = "";

            [RegularExpression(@"^$|^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Phone Number is required.")]
            public string Phone { get; set; } = "";

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = "";
        }
    }
}