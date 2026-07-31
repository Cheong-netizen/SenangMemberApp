using SenangMemberApp.Services;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SenangMemberApp.Shared.ApiClient
{
    public abstract class BaseAC
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;
        private readonly ITokenService _tokenService;
        private readonly NavigationManager? _navigationManager;
        private readonly IShopState? _shopState;

        protected BaseAC(HttpClient httpClient, ITokenService tokenService, NavigationManager? navigationManager = null, IShopState? shopState = null)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _navigationManager = navigationManager;
            _shopState = shopState;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        protected async Task<bool> ValidateUserAccessTokenAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("[AUTH] No user access token found.");
                return false;
            }

            if (JwtParser.IsTokenExpired(token))
            {
                Debug.WriteLine("[SESSION EXPIRED] User access token is expired. Triggering logout...");
                await TriggerSessionExpiredLogoutAsync();
                return false;
            }
            return true;
        }

        protected async Task<bool> ValidateCompanyAccessTokenAsync()
        {
            var companyToken = await _tokenService.GetCompanyTokenAsync();
            if (string.IsNullOrEmpty(companyToken) || JwtParser.IsTokenExpired(companyToken))
            {
                Debug.WriteLine("[SESSION EXPIRED] Company access token is missing or expired.");
                await _tokenService.ClearCompanyAsync();
                return false;
            }
            return true;
        }

        protected async Task TriggerSessionExpiredLogoutAsync()
        {
            try
            {
                await _tokenService.ClearAsync();
                if (_shopState != null)
                {
                    await _shopState.ResetStateAsync();
                }
                if (_navigationManager != null)
                {
                    var currentUri = _navigationManager.Uri;
                    if (!currentUri.EndsWith("/") && !currentUri.EndsWith("/login", StringComparison.OrdinalIgnoreCase))
                    {
                        _navigationManager.NavigateTo("/");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LOGOUT ERROR] Exception during session expired logout: {ex.Message}");
            }
        }

        private async Task PrepareHeaderAsync()
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH ERROR] JS Interop not ready: {ex.Message}");
            }
        }

        protected async Task<TResponse?> GetAsync<TResponse>(string endpoint)
        {
            try
            {
                if (!await ValidateUserAccessTokenAsync())
                {
                    return default;
                }

                await PrepareHeaderAsync();
                var response = await _httpClient.GetAsync(endpoint);
                return await HandleResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                LogException("GET", endpoint, ex);
                return default;
            }
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                if (!await ValidateUserAccessTokenAsync())
                {
                    return default;
                }

                // 1. Fetch token fresh from storage
                var token = await _tokenService.GetTokenAsync();

                // 2. Create the request message manually to ensure headers are clean
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Debug.WriteLine($"[API] Attached Token: {token.Substring(0, Math.Min(10, token.Length))}...");
                }

                var jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                Debug.WriteLine($"[API DEBUG] Endpoint: {endpoint}");
                Debug.WriteLine($"[API DEBUG] Payload: {jsonPayload}");

                request.Content = JsonContent.Create(data, options: _jsonOptions);

                // 3. Send
                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                LogException("POST", endpoint, ex);
                return default;
            }
        }

        protected async Task<TResponse?> DeleteAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                if (!await ValidateUserAccessTokenAsync())
                {
                    return default;
                }

                // 1. Fetch token
                var token = await _tokenService.GetTokenAsync();

                // 2. Create the DELETE request message
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // 3. Attach the JSON body to the DELETE request
                request.Content = JsonContent.Create(data, options: _jsonOptions);

                // 4. Send
                var response = await _httpClient.SendAsync(request);

                return await HandleResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                LogException("DELETE", endpoint, ex);
                return default;
            }
        }

        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                if (!await ValidateUserAccessTokenAsync())
                {
                    return default;
                }

                // 1. Fetch token fresh
                var token = await _tokenService.GetTokenAsync();

                // 2. Create manual request to stay thread-safe
                var request = new HttpRequestMessage(HttpMethod.Put, endpoint);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // 3. Attach content
                request.Content = JsonContent.Create(data, options: _jsonOptions);

                Debug.WriteLine(data);
                Debug.WriteLine(request);

                // 4. Send
                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                LogException("PUT", endpoint, ex);
                return default;
            }
        }

        // --- Helper Methods ---
        private async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Debug.WriteLine("[API ERROR 401] Unauthorized. Triggering session expired logout.");
                await TriggerSessionExpiredLogoutAsync();
                return default;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[API ERROR] {response.StatusCode}: {errorContent}");
                return default;
            }
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        private void LogException(string method, string endpoint, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HTTP {method} EXCEPTION] {endpoint}: {ex.Message}");
        }

        protected async Task<TResponse?> CompanyPostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                if (!await ValidateCompanyAccessTokenAsync())
                {
                    return default;
                }

                // 1. Fetch token fresh from storage
                var token = await _tokenService.GetCompanyTokenAsync();

                // 2. Create the request message manually to ensure headers are clean
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Debug.WriteLine($"[API] Attached Token: {token.Substring(0, Math.Min(10, token.Length))}...");
                }

                var jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                Debug.WriteLine($"[API DEBUG] Endpoint: {endpoint}");
                Debug.WriteLine($"[API DEBUG] Payload: {jsonPayload}");

                request.Content = JsonContent.Create(data, options: _jsonOptions);

                // 3. Send
                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                LogException("POST", endpoint, ex);
                return default;
            }
        }

        protected async Task<TResponse?> PostAnonymousAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                // 1. Create the request message manually without any authorization headers
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

                var jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                Debug.WriteLine($"[API DEBUG - NO AUTH] Endpoint: {endpoint}");
                Debug.WriteLine($"[API DEBUG - NO AUTH] Payload: {jsonPayload}");

                request.Content = JsonContent.Create(data, options: _jsonOptions);

                // 2. Send the request
                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                LogException("POST (No Auth)", endpoint, ex);
                return default;
            }
        }
    }
}
