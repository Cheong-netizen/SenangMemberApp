using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using SenangMemberApp.Shared.Services.IService;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class AppVersionService : IAppVersionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _versionCheckUrl = "https://raw.githubusercontent.com/EBI-Softwares/appsVersion/main/SenangMemberApp.json";
        private readonly string _platform;
        private readonly string _currentVersion;
        private AppVersionInfo? _cachedVersionInfo;

        public AppVersionService(HttpClient httpClient, string platform, string version)
        {
            _httpClient = httpClient;
            _platform = platform;
            _currentVersion = !string.IsNullOrWhiteSpace(version) ? version : GetCurrentVersionFromAssembly();

            System.Diagnostics.Debug.WriteLine($"=== Platform: {_platform}, Version: {_currentVersion} ===");
        }

        public async Task<AppVersionInfo?> GetLatestVersionInfoAsync()
        {
            try
            {
                // Return cached version if available
                if (_cachedVersionInfo != null)
                {
                    System.Diagnostics.Debug.WriteLine("=== Returning cached version info ===");
                    return _cachedVersionInfo;
                }

                System.Diagnostics.Debug.WriteLine($"=== Fetching from: {_versionCheckUrl} ===");
                var response = await _httpClient.GetAsync(_versionCheckUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"=== Response received: {content} ===");
                    _cachedVersionInfo = await response.Content.ReadFromJsonAsync<AppVersionInfo>();
                    return _cachedVersionInfo;
                }
                System.Diagnostics.Debug.WriteLine($"=== Response Status: {response.StatusCode} ===");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== Error fetching version info: {ex.Message} ===");
                return null;
            }
        }

        public async Task<bool> IsUpdateAvailableAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== IsUpdateAvailableAsync START ===");

                var versionInfo = await GetLatestVersionInfoAsync();
                if (versionInfo == null)
                {
                    System.Diagnostics.Debug.WriteLine("=== Version info is null ===");
                    return false;
                }

                var currentVersion = GetCurrentVersion();
                var latestVersion = GetLatestVersionForPlatform(versionInfo);

                System.Diagnostics.Debug.WriteLine($"=== Current: {currentVersion}, Latest: {latestVersion} ===");

                if (string.IsNullOrEmpty(latestVersion))
                {
                    System.Diagnostics.Debug.WriteLine("=== Latest version is empty ===");
                    return false;
                }

                var result = CompareVersions(latestVersion, currentVersion) > 0;
                System.Diagnostics.Debug.WriteLine($"=== Update available: {result} ===");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== Error in IsUpdateAvailableAsync: {ex.Message} ===");
                return false;
            }
        }

        public async Task<string> GetUpdateUrlAsync()
        {
            try
            {
                var versionInfo = await GetLatestVersionInfoAsync();

                if (string.Equals(_platform, "Android", StringComparison.OrdinalIgnoreCase))
                    return !string.IsNullOrEmpty(versionInfo?.android_update_url) ? versionInfo.android_update_url : "https://play.google.com/store/apps/details?id=com.ebi.senangmemberapp";
                else if (string.Equals(_platform, "iOS", StringComparison.OrdinalIgnoreCase))
                    return !string.IsNullOrEmpty(versionInfo?.ios_update_url) ? versionInfo.ios_update_url : "https://apps.apple.com/my/app/senang-app/id6791389839";
                else
                    return string.Empty;
            }
            catch
            {
                if (string.Equals(_platform, "Android", StringComparison.OrdinalIgnoreCase))
                    return "https://play.google.com/store/apps/details?id=com.ebi.senangmemberapp";
                else if (string.Equals(_platform, "iOS", StringComparison.OrdinalIgnoreCase))
                    return "https://apps.apple.com/my/app/senang-app/id6791389839";
                else
                    return string.Empty;
            }
        }

        public string GetCurrentVersion()
        {
            return _currentVersion;
        }

        public string GetPlatform()
        {
            return _platform;
        }

        private string GetCurrentVersionFromAssembly()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    var version = assembly.GetName().Version;
                    if (version != null && version.Major > 0)
                    {
                        var v = $"{version.Major}.{version.Minor}";
                        System.Diagnostics.Debug.WriteLine($"=== Version from assembly: {v} ===");
                        return v;
                    }
                }

                return "1.0";
            }
            catch
            {
                return "1.0";
            }
        }

        private string GetLatestVersionForPlatform(AppVersionInfo versionInfo)
        {
            if (string.Equals(_platform, "Android", StringComparison.OrdinalIgnoreCase))
                return versionInfo.android_latest_version;
            else if (string.Equals(_platform, "iOS", StringComparison.OrdinalIgnoreCase))
                return versionInfo.ios_latest_version;
            else
                return string.Empty;
        }

        private int CompareVersions(string version1, string version2)
        {
            if (string.IsNullOrEmpty(version1) || string.IsNullOrEmpty(version2))
                return 0;

            try
            {
                var v1Parts = version1.Trim().Split('.');
                var v2Parts = version2.Trim().Split('.');

                for (int i = 0; i < Math.Max(v1Parts.Length, v2Parts.Length); i++)
                {
                    int v1 = i < v1Parts.Length && int.TryParse(v1Parts[i], out int v1Result) ? v1Result : 0;
                    int v2 = i < v2Parts.Length && int.TryParse(v2Parts[i], out int v2Result) ? v2Result : 0;

                    if (v1 != v2)
                        return v1.CompareTo(v2);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
