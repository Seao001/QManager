using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace QManager.ViewModels
{
    public partial class DisplayViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> _resolutions = new() { "1920x1080", "1600x900", "1366x768", "1280x720", "1024x768" };

        [ObservableProperty]
        private string _selectedResolution = "1280x720";

        [ObservableProperty]
        private bool _isFullscreen;

        public DisplayViewModel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                _isFullscreen = desktop.MainWindow.WindowState == WindowState.FullScreen;
                _selectedResolution = $"{(int)desktop.MainWindow.Width}x{(int)desktop.MainWindow.Height}";
                if (!_resolutions.Contains(_selectedResolution)) _resolutions.Insert(0, _selectedResolution);

                // Asigurăm că decorațiunile de sistem corespund stării curente de fullscreen la inițializarea ViewModel-ului
                desktop.MainWindow.SystemDecorations = _isFullscreen ? SystemDecorations.None : SystemDecorations.Full;
            }
        }

        [RelayCommand]
        private void ApplySettings()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                var window = desktop.MainWindow;
                
                if (IsFullscreen)
                {
                    window.SystemDecorations = SystemDecorations.None; // Fără decorațiuni în modul ecran complet
                    window.WindowState = WindowState.FullScreen;
                }
                else
                {
                    window.WindowState = WindowState.Normal;
                    window.SystemDecorations = SystemDecorations.Full; // Cu decorațiuni pentru modul fereastră, permițând mutarea
                    var dim = SelectedResolution.Split('x');
                    if (dim.Length == 2 && double.TryParse(dim[0], out var w) && double.TryParse(dim[1], out var h))
                    {
                        window.Width = w;
                        window.Height = h;
                        if (window.Screens.Primary is { } primaryScreen)
                        {
                            window.Position = new PixelPoint((int)(primaryScreen.WorkingArea.Width - w) / 2, (int)(primaryScreen.WorkingArea.Height - h) / 2); // Centrează fereastra
                        }
                    }
                }
            }
        }
    }
}