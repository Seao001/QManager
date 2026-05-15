using System;
using System.IO;
using System.Text.Json;

namespace QManager.Service
{
    public static class SettingsManager
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QManager");
        private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "appsettings.json");

        private static AppSettings _currentSettings = new AppSettings();

        public static AppSettings CurrentSettings => _currentSettings;

        public static void LoadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                _currentSettings = new AppSettings(); // Default settings
                SaveSettings();
                return;
            }

            try
            {
                var json = File.ReadAllText(SettingsFilePath);
                var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json);
                if (loadedSettings != null)
                {
                    _currentSettings = loadedSettings;
                }
                else
                {
                    _currentSettings = new AppSettings();
                }
            }
            catch
            {
                _currentSettings = new AppSettings(); // Fallback to default on error
            }
        }

        public static void SaveSettings()
        {
            Directory.CreateDirectory(SettingsDirectory);
            var json = JsonSerializer.Serialize(_currentSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }

        public class AppSettings
        {
            public string Language { get; set; } = "English"; // Default language
            public bool IsDarkMode { get; set; } = false; // Default theme
        }
    }
}