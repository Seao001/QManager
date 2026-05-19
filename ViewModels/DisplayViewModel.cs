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
        private List<string> _resolutions = new() { "1920x1080", "1600x900", "1440x900", "1366x768", "1280x720", "1024x768" };

        [ObservableProperty]
        private string _selectedResolution;

        [ObservableProperty]
        private bool _isFullscreen;

        public DisplayViewModel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                _isFullscreen = desktop.MainWindow.WindowState == WindowState.FullScreen;
                _selectedResolution = $"{(int)desktop.MainWindow.Width}x{(int)desktop.MainWindow.Height}";
                if (!_resolutions.Contains(_selectedResolution)) _resolutions.Insert(0, _selectedResolution);
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
                    window.WindowState = WindowState.FullScreen;
                    window.SystemDecorations = SystemDecorations.None;
                }
                else
                {
                    var dim = SelectedResolution.Split('x');
                    if (dim.Length == 2 && double.TryParse(dim[0], out var w) && double.TryParse(dim[1], out var h))
                    {
                        window.WindowState = WindowState.Normal;
                        window.Width = w;
                        window.Height = h;
                        window.SystemDecorations = SystemDecorations.Full;
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