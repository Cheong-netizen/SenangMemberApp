using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Services.IService
{
    public class ThemeOption
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ThemeClass { get; set; } = string.Empty;
    }

    public interface IThemeService
    {
        string CurrentColor { get; }
        string CurrentThemeClass { get; }
        IReadOnlyList<ThemeOption> AvailableThemes { get; }
        event Action? OnThemeChanged;
        Task InitializeThemeAsync();
        Task SetThemeColorAsync(string color);
        string GetThemeClass(string color);
    }
}
