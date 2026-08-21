namespace SenangMemberApp.Shared.Infrastructure.Firebase;

/// <summary>
/// Used by the web host, which does not participate in native Android FCM registration.
/// </summary>
public sealed class NullPushDeviceRegistrationService : IPushDeviceRegistrationService
{
    public Task<PushDeviceOperationResult> RegisterCurrentDeviceAsync(
        bool force = false,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(PushDeviceOperationResult.Skipped("native_push_not_available"));

    public Task<PushDeviceOperationResult> UpdatePreferredLanguageAsync(
        string languageCode,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(PushDeviceOperationResult.Skipped("native_push_not_available"));

    public Task<PushDeviceOperationResult> UnregisterCurrentDeviceAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(PushDeviceOperationResult.Skipped("native_push_not_available"));

    public Task<PushNotificationTestResult?> SendTestNotificationAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult<PushNotificationTestResult?>(null);
}
