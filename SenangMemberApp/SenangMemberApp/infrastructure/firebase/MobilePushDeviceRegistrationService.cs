using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using SenangMemberApp.Shared.Infrastructure.Firebase;
using SenangMemberApp.Shared.Services.IService;

namespace SenangMemberApp.Infrastructure.Firebase;

/// <summary>
/// Registers this app installation with the trusted AppointmentApp server.
/// Firebase Admin credentials must remain on that server and never enter the mobile app.
/// </summary>
public sealed class MobilePushDeviceRegistrationService : IPushDeviceRegistrationService
{
    public const string HttpClientName = "PushDeviceApi";

    private const string InstallationIdKey = "firebase_installation_device_id";
    private const string PreferredLanguageKey = "firebase_preferred_language";
    private const string LastRegistrationFingerprintKey = "firebase_last_registration_fingerprint";
    private const string LastRegistrationUtcKey = "firebase_last_registration_utc";
    private static readonly TimeSpan RegistrationRefreshInterval = TimeSpan.FromHours(12);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _registrationLock = new(1, 1);

    public MobilePushDeviceRegistrationService(
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
    }

    public async Task<PushDeviceOperationResult> RegisterCurrentDeviceAsync(
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        await _registrationLock.WaitAsync(cancellationToken);
        try
        {
            var fcmToken = await FirebaseTokenStore.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(fcmToken))
            {
                return PushDeviceOperationResult.Skipped("fcm_token_unavailable");
            }

            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new PushDeviceOperationResult(false, false, null, "not_authenticated");
            }

            var installationId = GetOrCreateInstallationId();
            var languageCode = GetPreferredLanguageCode();
            var fingerprint = CreateFingerprint(
                $"{fcmToken}:{installationId}:{AppInfo.Current.VersionString}:{languageCode}:{accessToken}");
            if (!force && IsRecentSuccessfulRegistration(fingerprint))
            {
                return new PushDeviceOperationResult(true, true, 200, "already_registered");
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/push-devices")
            {
                Content = JsonContent.Create(new RegisterPushDeviceRequest(
                    fcmToken,
                    "android",
                    installationId,
                    AppInfo.Current.VersionString,
                    languageCode))
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClientFactory
                .CreateClient(HttpClientName)
                .SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                Preferences.Default.Set(LastRegistrationFingerprintKey, fingerprint);
                Preferences.Default.Set(LastRegistrationUtcKey, DateTimeOffset.UtcNow.ToString("O"));
                return new PushDeviceOperationResult(true, true, (int)response.StatusCode);
            }

            return new PushDeviceOperationResult(
                false,
                response.StatusCode != HttpStatusCode.Unauthorized,
                (int)response.StatusCode,
                response.StatusCode == HttpStatusCode.NotFound
                    ? "push_endpoint_not_deployed"
                    : "registration_rejected");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[FCM] Device registration failed: {ex.GetType().Name}");
            return new PushDeviceOperationResult(false, true, null, "network_or_client_error");
        }
        finally
        {
            _registrationLock.Release();
        }
    }

    public Task<PushDeviceOperationResult> UpdatePreferredLanguageAsync(
        string languageCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedLanguage = NormalizeLanguageCode(languageCode);
        var previousLanguage = Preferences.Default.Get(PreferredLanguageKey, string.Empty);
        Preferences.Default.Set(PreferredLanguageKey, normalizedLanguage);

        return RegisterCurrentDeviceAsync(
            force: !string.Equals(previousLanguage, normalizedLanguage, StringComparison.OrdinalIgnoreCase),
            cancellationToken: cancellationToken);
    }

    public async Task<PushDeviceOperationResult> UnregisterCurrentDeviceAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fcmToken = await FirebaseTokenStore.GetTokenAsync();
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(fcmToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                return PushDeviceOperationResult.Skipped("token_unavailable");
            }

            using var request = new HttpRequestMessage(HttpMethod.Delete, "api/push-devices")
            {
                Content = JsonContent.Create(new UnregisterPushDeviceRequest(
                    fcmToken,
                    GetOrCreateInstallationId()))
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClientFactory
                .CreateClient(HttpClientName)
                .SendAsync(request, cancellationToken);

            return new PushDeviceOperationResult(
                response.IsSuccessStatusCode,
                response.StatusCode != HttpStatusCode.Unauthorized,
                (int)response.StatusCode,
                response.IsSuccessStatusCode ? null : "unregistration_rejected");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[FCM] Device unregistration failed: {ex.GetType().Name}");
            return new PushDeviceOperationResult(false, true, null, "network_or_client_error");
        }
        finally
        {
            Preferences.Default.Remove(LastRegistrationFingerprintKey);
            Preferences.Default.Remove(LastRegistrationUtcKey);
        }
    }

    public async Task<PushNotificationTestResult?> SendTestNotificationAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/push-devices/test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClientFactory
                .CreateClient(HttpClientName)
                .SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PushNotificationTestResult>(
                cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[FCM] Test notification failed: {ex.GetType().Name}");
            return null;
        }
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ITokenService>().GetTokenAsync();
    }

    private static string GetOrCreateInstallationId()
    {
        var installationId = Preferences.Default.Get(InstallationIdKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(installationId))
        {
            return installationId;
        }

        installationId = Guid.NewGuid().ToString("N");
        Preferences.Default.Set(InstallationIdKey, installationId);
        return installationId;
    }

    private static string GetPreferredLanguageCode() =>
        NormalizeLanguageCode(Preferences.Default.Get(
            PreferredLanguageKey,
            CultureInfo.CurrentUICulture.Name));

    private static string NormalizeLanguageCode(string? languageCode)
    {
        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            if (languageCode.Equals("ms", StringComparison.OrdinalIgnoreCase) ||
                languageCode.StartsWith("ms-", StringComparison.OrdinalIgnoreCase))
            {
                return "ms-MY";
            }

            if (languageCode.Equals("zh", StringComparison.OrdinalIgnoreCase) ||
                languageCode.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
            {
                return "zh-CN";
            }
        }

        return "en-US";
    }

    private static bool IsRecentSuccessfulRegistration(string fingerprint)
    {
        if (Preferences.Default.Get(LastRegistrationFingerprintKey, string.Empty) != fingerprint)
        {
            return false;
        }

        return DateTimeOffset.TryParse(
                   Preferences.Default.Get(LastRegistrationUtcKey, string.Empty),
                   out var registeredAt) &&
               DateTimeOffset.UtcNow - registeredAt < RegistrationRefreshInterval;
    }

    private static string CreateFingerprint(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private sealed record RegisterPushDeviceRequest(
        string Token,
        string Platform,
        string DeviceId,
        string AppVersion,
        string LanguageCode);

    private sealed record UnregisterPushDeviceRequest(string Token, string DeviceId);
}
