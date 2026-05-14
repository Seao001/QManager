using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.Service;
using Avalonia;
using Avalonia.Styling;

namespace QManager.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isDarkMode;

        public SettingsViewModel()
        {
            // Inițializăm starea Toggle-ului cu tema curentă a aplicației
            _isDarkMode = ThemeManager.LoadSavedTheme();
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            // Schimbăm tema la nivel global și salvăm preferința
            ThemeManager.SwitchToTheme(value);
        }

        [RelayCommand]
        private void SaveSettings()
        {
            // Logica de salvare
        }
    }
}