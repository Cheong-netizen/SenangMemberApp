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
            ["CLR"] = "CLR"
        };

        private static readonly Dictionary<string, string> MsTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MalaysiaCountry"] = "Malaysia",
            ["SingaporeCountry"] = "Singapura",
            ["Save"] = "Simpan",
            ["Cancel"] = "Batal",
            ["Close"] = "Tutup",
            ["Phone"] = "Nombor Telefon",
            ["CLR"] = "PADAM"
        };

        private static readonly Dictionary<string, string> ZhTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MalaysiaCountry"] = "马来西亚",
            ["SingaporeCountry"] = "新加坡",
            ["Save"] = "保存",
            ["Cancel"] = "取消",
            ["Close"] = "关闭",
            ["Phone"] = "电话号码",
            ["CLR"] = "清除"
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
