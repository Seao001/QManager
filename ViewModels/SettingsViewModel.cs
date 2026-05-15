using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.Service;
using Avalonia;
using Avalonia.Styling;
using QManager.View;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Threading;

namespace QManager.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private Bitmap? _profileImage;

        public SettingsViewModel()
        {
            // Inițializăm starea Toggle-ului cu tema curentă a aplicației
            _isDarkMode = ThemeManager.LoadSavedTheme();

            LoadProfileImage();
            // Ne abonăm la evenimentul global de schimbare a pozei pentru a actualiza UI-ul instant
            SessionState.ProfilePhotoChanged += (s, e) => LoadProfileImage();
        }

        private void LoadProfileImage()
        {
            var path = SessionState.ProfilePhotoPath;
            if (!string.IsNullOrEmpty(path) && File.Exists(path) )
            {
                try
                {
                    // Încărcăm Bitmap-ul pe firul principal de UI pentru a asigura stabilitatea Binding-ului
                    Dispatcher.UIThread.Post(() =>
                    {
                        ProfileImage = new Bitmap(path);
                    });
                }
                catch { ProfileImage = null; }
            }
            else
            {
                ProfileImage = null;
            }
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