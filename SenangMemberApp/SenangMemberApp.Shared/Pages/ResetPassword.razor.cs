using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Pages
{
    public partial class ResetPassword
    {
        [Inject]
        private HttpClient HttpClient { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private string phoneNumber = "60183208832";
        private string inputOtpCode = "";
        private string generatedOtpCode = "";
        private bool isCodeSent = false;
        private bool isLoading = false;
        private bool isSuccess = false;
        private string errorMessage = "";
        private string successMessage = "";

        private const string GreenApiUrl = "https://7105.api.greenapi.com/waInstance7105472363/sendMessage/5a7db8f511c24d7abbefd0e2cec36ba50c07b35615fe44c19d";

        private async Task HandleSendCode()
        {
            errorMessage = "";
            successMessage = "";

            if (string.IsNullOrWhiteSpace(phoneNumber))
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
                string cleanedPhone = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "").Trim();
                if (!cleanedPhone.EndsWith("@c.us"))
                {
                    cleanedPhone = $"{cleanedPhone}@c.us";
                }

                var payload = new
                {
                    chatId = cleanedPhone,
                    message = $"Your SenangMember verification code is: {generatedOtpCode}",
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
                    successMessage = $"A 6-digit verification code has been sent to your WhatsApp ({phoneNumber}).";
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
            isCodeSent = false;
            await HandleSendCode();
        }

        private async Task HandleVerifyCode()
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
            isSuccess = true;
            successMessage = "Verification successful! Password reset complete. Redirecting to login page...";

            StateHasChanged();

            await Task.Delay(1500);

            NavigationManager.NavigateTo("/", replace: true);
        }
    }
}
