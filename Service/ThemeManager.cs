using Avalonia;
using Avalonia.Styling;
using System;
using System.IO;
using System.Text.Json;

namespace QManager.Service
{
    public static class ThemeManager
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QManager");
        private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "theme_settings.json");

        public static void Initialize()
        {
            ApplyTheme(LoadSavedTheme());
        }

        public static void SwitchToTheme(bool isDark)
        {
            ApplyTheme(isDark);
            SaveTheme(isDark);
        }

        private static void ApplyTheme(bool isDark)
        {
            if (Application.Current != null)
                Application.Current.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        public static bool LoadSavedTheme()
        {
            if (!File.Exists(SettingsFilePath)) return false;
            try
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<ThemeSettings>(json);
                return settings?.IsDarkMode ?? false;
            }
            catch { return false; }
        }

        private static void SaveTheme(bool isDark)
        {
            Directory.CreateDirectory(SettingsDirectory);
            var json = JsonSerializer.Serialize(new ThemeSettings { IsDarkMode = isDark });
            File.WriteAllText(SettingsFilePath, json);
        }

        private class ThemeSettings { public bool IsDarkMode { get; set; } }
    }
}