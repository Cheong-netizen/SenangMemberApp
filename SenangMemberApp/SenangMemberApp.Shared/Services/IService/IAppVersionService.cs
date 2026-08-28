using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Services.IService
{
    public class AppVersionInfo
    {
        public string ios_latest_version { get; set; } = string.Empty;
        public string android_latest_version { get; set; } = string.Empty;
        public string android_update_url { get; set; } = string.Empty;
        public string ios_update_url { get; set; } = string.Empty;
    }

    public interface IAppVersionService
    {
        Task<AppVersionInfo?> GetLatestVersionInfoAsync();
        Task<bool> IsUpdateAvailableAsync();
        Task<string> GetUpdateUrlAsync();
        string GetCurrentVersion();
        string GetPlatform();
    }
}
