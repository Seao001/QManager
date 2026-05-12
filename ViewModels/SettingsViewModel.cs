using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.Models;
using QManager.Service;

namespace QManager.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ThemeManager _themeManager = new ThemeManager();

        [ObservableProperty]
        private bool _isDarkMode;

        partial void OnIsDarkModeChanged(bool value)
        {
            _themeManager.SwitchToTheme(value);
        }

        [RelayCommand]
        private void SaveSettings()
        {
            // Logica de salvare
        }
    }
}