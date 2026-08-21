namespace SenangMemberApp.Shared.Infrastructure.Firebase;

public interface IPushDeviceRegistrationService
{
    Task<PushDeviceOperationResult> RegisterCurrentDeviceAsync(
        bool force = false,
        CancellationToken cancellationToken = default);

    Task<PushDeviceOperationResult> UpdatePreferredLanguageAsync(
        string languageCode,
        CancellationToken cancellationToken = default);

    Task<PushDeviceOperationResult> UnregisterCurrentDeviceAsync(
        CancellationToken cancellationToken = default);

    Task<PushNotificationTestResult?> SendTestNotificationAsync(
        CancellationToken cancellationToken = default);
}

public sealed record PushDeviceOperationResult(
    bool Succeeded,
    bool IsAuthenticated,
    int? StatusCode = null,
    string? DiagnosticCode = null)
{
    public static PushDeviceOperationResult Skipped(string code) =>
        new(false, true, null, code);
}

public sealed record PushNotificationTestResult(
    int TargetDeviceCount,
    int SuccessfulDeliveryCount,
    int FailedDeliveryCount,
    int InvalidTokenCount,
    int RetryableFailureCount);
