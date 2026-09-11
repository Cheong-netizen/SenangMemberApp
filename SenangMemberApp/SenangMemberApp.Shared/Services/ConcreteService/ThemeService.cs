using Microsoft.JSInterop;
using SenangMemberApp.Shared.Services.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SenangMemberApp.Shared.Services.ConcreteService
{
    public class ThemeService : IThemeService
    {
        private const string DefaultColor = "#B04E64";
        private const string StorageKey = "app_theme_color";
        private readonly IJSRuntime _js;
        private bool _isInitialized = false;

        public string CurrentColor { get; private set; } = DefaultColor;
        public string CurrentThemeClass => GetThemeClass(CurrentColor);
        public event Action? OnThemeChanged;

        public IReadOnlyList<ThemeOption> AvailableThemes { get; } = new List<ThemeOption>
        {
            new() { Name = "Default", Color = "#B04E64", ThemeClass = "theme-default" },
            new() { Name = "Blue",    Color = "#2196F3", ThemeClass = "theme-blue" },
            new() { Name = "Green",   Color = "#4CAF50", ThemeClass = "theme-green" },
            new() { Name = "Orange",  Color = "#FF6B35", ThemeClass = "theme-orange" },
            new() { Name = "Yellow",  Color = "#E8A317", ThemeClass = "theme-yellow" },
            new() { Name = "Purple",  Color = "#8E5A7A", ThemeClass = "theme-purple" },
            new() { Name = "Pink",    Color = "#D87093", ThemeClass = "theme-pink" },
            new() { Name = "Teal",    Color = "#008080", ThemeClass = "theme-teal" },
            new() { Name = "Brown",   Color = "#795548", ThemeClass = "theme-brown" },
            new() { Name = "Slate",   Color = "#607D8B", ThemeClass = "theme-slate" },
        };

        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitializeThemeAsync()
        {
            if (_isInitialized) return;

            try
            {
                var savedColor = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
                if (!string.IsNullOrWhiteSpace(savedColor))
                {
                    CurrentColor = savedColor;
                }
                else
                {
                    CurrentColor = DefaultColor;
                }

                await ApplyThemeToDomAsync(CurrentColor);
                _isInitialized = true;
                OnThemeChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ThemeService] InitializeThemeAsync error: {ex.Message}");
            }
        }

        public async Task SetThemeColorAsync(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return;

            try
            {
                CurrentColor = color;
                await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, color);
                await ApplyThemeToDomAsync(color);
                OnThemeChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ThemeService] SetThemeColorAsync error: {ex.Message}");
            }
        }

        public string GetThemeClass(string color)
        {
            return color?.ToLowerInvariant() switch
            {
                "#b04e64" => "theme-default",
                "#2196f3" => "theme-blue",
                "#4caf50" => "theme-green",
                "#ff6b35" => "theme-orange",
                "#e8a317" => "theme-yellow",
                "#8e5a7a" => "theme-purple",
                "#d87093" => "theme-pink",
                "#008080" => "theme-teal",
                "#795548" => "theme-brown",
                "#607d8b" => "theme-slate",
                _ => "theme-default"
            };
        }

        private async Task ApplyThemeToDomAsync(string color)
        {
            try
            {
                await _js.InvokeVoidAsync("window.applyThemeStyles", color);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ThemeService] ApplyThemeToDomAsync error: {ex.Message}");
            }
        }
    }
}
