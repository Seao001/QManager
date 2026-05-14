using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using QManager.Service;

namespace QManager.View
{
    public partial class MainWindow : Window
    {
        private Border _menuPanel = null!;
        private Button _menuBackdrop = null!;
        private TextBlock _menuUserText = null!;
        private TransitioningContentControl _mainContentPresenter = null!;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            this.SystemDecorations = SystemDecorations.None;
            this.WindowState = WindowState.FullScreen;
            ResolveControls();
            AttachViewNavigation();

            _menuUserText.Text = string.IsNullOrWhiteSpace(SessionState.Username)
                ? "Admin Name"
                : SessionState.Username;

            SetMainContent(new BankView());
            HideMenu();
        }

        private void ResolveControls()
        {
            _menuPanel = GetRequiredControl<Border>("MenuPanel");
            _menuBackdrop = GetRequiredControl<Button>("MenuBackdrop");
            _mainContentPresenter = GetRequiredControl<TransitioningContentControl>("MainContentPresenter");
            _menuUserText = GetRequiredControl<TextBlock>("MenuUserText");
        }

        private T GetRequiredControl<T>(string name) where T : Control =>
            this.FindControl<T>(name) ?? throw new InvalidOperationException($"{typeof(T).Name} '{name}' was not found in MainWindow.axaml.");

        private void AttachViewNavigation()
        {

            _menuBackdrop.Click += HideMenu_Click;

            _mainContentPresenter.PointerPressed += (s, e) => 
            { 
                if (_menuPanel.IsVisible) HideMenu(); 
                
                // Forțează pierderea focusului (blur) de pe orice TextBox activ
                this.Focus();
            };
        }

        private void OnNavigationRequested(object? sender, NavigationRequestEventArgs e)
        {
            switch (e.Target)
            {
                case "Bank":
                    SetMainContent(new BankView());
                    break;
                case "Casier":
                case "Talon":
                    SetMainContent(new TalonView());
                    break;
                case "Dashboard":
                    SetMainContent(new DashboardView());
                    break;
                case "Settings":
                    SetMainContent(new SettingsView());
                    break;
                case "SignOut":
                    SessionState.SignOut();
                    WindowNavigator.Open<LoginWindow>(this);
                    break;
                case "Exit":
                    this.Close();
                    break;
            }
        }

        private void SetMainContent(UserControl view)
        {
            if (view is DashboardView dv) dv.NavigationRequested += OnNavigationRequested;
            if (view is TalonView tv) tv.NavigationRequested += OnNavigationRequested;
            if (view is SettingsView sv) sv.NavigationRequested += OnNavigationRequested;
            if (view is BankView bv) { /* bv has no NavRequested in current code but could */ }

            _mainContentPresenter.Content = view;
            HideMenu();
        }

        private void ToggleMenu_Click(object? sender, RoutedEventArgs e)
        {
            var isOpen = !_menuPanel.IsVisible;
            _menuPanel.IsVisible = isOpen;
            _menuBackdrop.IsVisible = isOpen;
        }

        private void HideMenu()
        {
            _menuPanel.IsVisible = false;
            _menuBackdrop.IsVisible = false;
        }

        private void HideMenu_Click(object? sender, RoutedEventArgs e) => HideMenu();

        private void MenuPanel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // Prevenim închiderea meniului când se dă click direct pe el (Handled = true)
            e.Handled = true;
        }

        private void OpenBank_Click(object? sender, RoutedEventArgs e) => SetMainContent(new BankView());
        private void OpenTalon_Click(object? sender, RoutedEventArgs e) => SetMainContent(new TalonView());
        private void OpenDashboard_Click(object? sender, RoutedEventArgs e) => SetMainContent(new DashboardView());
        private void OpenSettings_Click(object? sender, RoutedEventArgs e) => SetMainContent(new SettingsView());
        private void ExitApp_Click(object? sender, RoutedEventArgs e) => this.Close();
    }
}
