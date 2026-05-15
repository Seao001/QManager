using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QManager.Service;
using Avalonia.Threading;
using QManager.ViewModels;
using Avalonia.Media.Imaging;

namespace QManager.View
{
    public partial class SettingsView : UserControl
    {
        private TextBlock _settingsStatusText = null!;
        private TextBlock _currentAdminNameText = null!;

        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public SettingsView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new SettingsViewModel();
            _settingsStatusText = this.FindControl<TextBlock>("SettingsStatusText")!;
            _currentAdminNameText = this.FindControl<TextBlock>("CurrentAdminNameText")!;

            UpdateUsernameDisplay();

            // Ne abonăm la schimbările de nume pentru a actualiza UI-ul instant
            SessionState.UsernameChanged += (s, e) => UpdateUsernameDisplay();
            LocalizationService.Instance.LanguageChanged += (s, e) => UpdateUsernameDisplay();
        }

        private void UpdateUsernameDisplay()
        {
            _currentAdminNameText.Text = string.IsNullOrWhiteSpace(SessionState.Username)
                ? LocalizationService.Instance["AdminName"]
                : SessionState.Username;
        }


        private void ChangeAdminName_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("ChangeName"));
        }

        private void ChangePhoto_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("ChangePhoto"));
        }

        private void ChangePassword_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("PasswordChange"));
        }

        private void ChangeLanguage_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("LanguageSelection"));
        }

        private void AboutUs_Click(object? sender, RoutedEventArgs e)
        {
            _settingsStatusText.Text = "QManager helps manage bank queue tickets and service rooms.";
        }

        private void DisplaySettings_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("Display"));
        }

        private void SignOut_Click(object? sender, RoutedEventArgs e)
        {
            SessionState.SignOut();
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("SignOut"));
        }
    }
}
