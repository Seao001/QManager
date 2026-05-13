using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QManager.Service;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class SettingsView : UserControl
    {
        private TextBlock _settingsStatusText = null!;

        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public SettingsView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new SettingsViewModel();
            _settingsStatusText = this.FindControl<TextBlock>("SettingsStatusText")!;
        }


        private void ChangeAdminName_Click(object? sender, RoutedEventArgs e)
        {
            _settingsStatusText.Text = "Admin name change panel will be connected to the profile repository.";
        }

        private void ChangePhoto_Click(object? sender, RoutedEventArgs e)
        {
            _settingsStatusText.Text = "Photo picker will be connected here.";
        }

        private void ChangePassword_Click(object? sender, RoutedEventArgs e)
        {
            _settingsStatusText.Text = "Password change flow will be connected here.";
        }

        private void ChangeLanguage_Click(object? sender, RoutedEventArgs e)
        {
            _settingsStatusText.Text = "Language switch is ready for localization wiring.";
        }

        private void AboutUs_Click(object? sender, RoutedEventArgs e)
        {
            _settingsStatusText.Text = "QManager helps manage bank queue tickets and service rooms.";
        }

        private void SignOut_Click(object? sender, RoutedEventArgs e)
        {
            SessionState.SignOut();
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("SignOut"));
        }
    }
}
