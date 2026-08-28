using System;
using System.Collections.Generic;
using System.Globalization;

namespace SenangMemberApp.Shared.Services
{
    public class LanguageService
    {
        private static readonly Dictionary<string, string> EnTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MalaysiaCountry"] = "Malaysia",
            ["SingaporeCountry"] = "Singapore",
            ["Save"] = "Save",
            ["Cancel"] = "Cancel",
            ["Close"] = "Close",
            ["Phone"] = "Phone Number",
            ["CLR"] = "CLR",
            ["UpdateAvailable"] = "Update Available",
            ["UpdateMessage"] = "A new version of the app is available. Please update to the latest version to continue.",
            ["UpdateNow"] = "Update Now"
        };

        private static readonly Dictionary<string, string> MsTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MalaysiaCountry"] = "Malaysia",
            ["SingaporeCountry"] = "Singapura",
            ["Save"] = "Simpan",
            ["Cancel"] = "Batal",
            ["Close"] = "Tutup",
            ["Phone"] = "Nombor Telefon",
            ["CLR"] = "PADAM",
            ["UpdateAvailable"] = "Kemas Kini Tersedia",
            ["UpdateMessage"] = "Versi baharu aplikasi telah tersedia. Sila kemas kini ke versi terkini untuk meneruskan.",
            ["UpdateNow"] = "Kemas Kini Sekarang"
        };

        private static readonly Dictionary<string, string> ZhTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MalaysiaCountry"] = "马来西亚",
            ["SingaporeCountry"] = "新加坡",
            ["Save"] = "保存",
            ["Cancel"] = "取消",
            ["Close"] = "关闭",
            ["Phone"] = "电话号码",
            ["CLR"] = "清除",
            ["UpdateAvailable"] = "发现新版本",
            ["UpdateMessage"] = "应用有新版本可用。请更新至最新版本以继续使用。",
            ["UpdateNow"] = "立即更新"
        };

        public string GetText(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            var culture = CultureInfo.CurrentUICulture.Name;

            if (culture.StartsWith("ms", StringComparison.OrdinalIgnoreCase))
            {
                if (MsTranslations.TryGetValue(key, out var msVal))
                    return msVal;
            }
            else if (culture.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
            {
                if (ZhTranslations.TryGetValue(key, out var zhVal))
                    return zhVal;
            }

            if (EnTranslations.TryGetValue(key, out var enVal))
                return enVal;

            return key;
        }

        public string this[string key] => GetText(key);
    }
}
