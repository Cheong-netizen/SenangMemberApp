using Microsoft.Maui.Storage;

namespace SenangMemberApp.Infrastructure.Firebase;

/// <summary>
/// Provides the FCM registration token for the current app installation.
/// Send this value to the application backend after a user signs in so the
/// backend can address notifications to this device.
/// </summary>
public static class FirebaseTokenStore
{
    private const string TokenPreferenceKey = "firebase_fcm_registration_token";
    private static string? _currentToken;

    public static string? CurrentToken =>
        _currentToken ?? Preferences.Default.Get<string?>(TokenPreferenceKey, null);

    public static async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrWhiteSpace(_currentToken))
        {
            return _currentToken;
        }

        try
        {
            _currentToken = await SecureStorage.Default.GetAsync(TokenPreferenceKey);
        }
        catch
        {
            // Secure storage can be temporarily unavailable before the app UI is initialized.
        }

        if (!string.IsNullOrWhiteSpace(_currentToken))
        {
            return _currentToken;
        }

        var legacyToken = Preferences.Default.Get<string?>(TokenPreferenceKey, null);
        if (!string.IsNullOrWhiteSpace(legacyToken))
        {
            await SaveAsync(legacyToken);
        }

        return _currentToken;
    }

    internal static async Task SaveAsync(string token)
    {
        _currentToken = token;

        try
        {
            await SecureStorage.Default.SetAsync(TokenPreferenceKey, token);
            Preferences.Default.Remove(TokenPreferenceKey);
        }
        catch
        {
            // Preserve registration across restarts if secure storage is temporarily unavailable.
            Preferences.Default.Set(TokenPreferenceKey, token);
        }
    }
}
