using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Threading;
using QManager.Service;

namespace QManager.View
{
    public partial class MainWindow : Window
    {
        private Border _menuPanel = null!;
        private Button _menuBackdrop = null!;
        private TextBlock _menuUserText = null!;
        private Image _menuAvatarImage = null!; // Adăugat: Controlul Image pentru avatarul din meniu
        private TransitioningContentControl _mainContentPresenter = null!;
        private UserControl? _currentContentView;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            this.SystemDecorations = SystemDecorations.None;
            this.WindowState = WindowState.FullScreen;
            ResolveControls();
            AttachViewNavigation();

            this.DataContext = this;
            UpdateMenuUserText();
            UpdateMenuAvatar(); // Adăugat: Actualizează avatarul la pornire
            LocalizationService.Instance.LanguageChanged += (s, e) => UpdateMenuUserText();
            SessionState.UsernameChanged += (s, e) => UpdateMenuUserText();
            SessionState.ProfilePhotoChanged += (s, e) => UpdateMenuAvatar(); // Adăugat: Abonează-te la schimbările fotografiei de profil

            SetMainContent(new TalonView());
            HideMenu();
        }

        private void UpdateMenuUserText()
        {
            _menuUserText.Text = string.IsNullOrWhiteSpace(SessionState.Username)
                ? LocalizationService.Instance["AdminName"]
                : SessionState.Username;
        }

        private void UpdateMenuAvatar() // Adăugat: Metodă pentru a actualiza imaginea avatarului din meniu
        {
            Dispatcher.UIThread.Post(() =>
            {
                var path = SessionState.ProfilePhotoPath;
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    try { _menuAvatarImage.Source = new Avalonia.Media.Imaging.Bitmap(path); }
                    catch { _menuAvatarImage.Source = null; }
                }
                else
                {
                    _menuAvatarImage.Source = null;
                }
            });
        }

        private void ResolveControls()
        {
            _menuPanel = GetRequiredControl<Border>("MenuPanel");
            _menuBackdrop = GetRequiredControl<Button>("MenuBackdrop");
            _mainContentPresenter = GetRequiredControl<TransitioningContentControl>("MainContentPresenter");
            _menuUserText = GetRequiredControl<TextBlock>("MenuUserText");
            _menuAvatarImage = GetRequiredControl<Image>("MenuAvatarImage"); // Adăugat: Rezolvă noul control Image
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
                    SetMainContent(new TalonView());
                    break;
                case "Casier":
                case "Talon":
                    SetMainContent(new BankView());
                    break;
                case "Dashboard":
                    SetMainContent(new DashboardView());
                    break;
                case "Settings":
                    SetMainContent(new SettingsView());
                    break;
                case "LanguageSelection":
                    SetMainContent(new LanguageSelectionView());
                    break;
                case "PasswordChange":
                    SetMainContent(new PasswordChangeView());
                    break;
                case "Display":
                    SetMainContent(new DisplayView());
                    break;
                case "ChangeName":
                    SetMainContent(new ChangeAdminNameView());
                    break;
                case "ChangePhoto":
                    SetMainContent(new ChangePhotoView());
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
            // Dezlipim evenimentele de pe view-ul vechi pentru a evita memory leaks
            if (_currentContentView != null)
            {
                if (_currentContentView is MainView mv) mv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is SettingsView sv) sv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is DisplayView dsv) dsv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is PasswordChangeView pcv) pcv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is LanguageSelectionView lsv) lsv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is TalonView tv) tv.NavigationRequested -= OnNavigationRequested; // Unsubscribe TalonView
                if (_currentContentView is BankView bv) bv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is ChangePhotoView cpv) cpv.NavigationRequested -= OnNavigationRequested;
                if (_currentContentView is ChangeAdminNameView canv) canv.NavigationRequested -= OnNavigationRequested;
            }

            _currentContentView = view;

            // Abonăm noul view la sistemul de navigare
            if (_currentContentView is MainView nmv) nmv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is SettingsView nsv) nsv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is DisplayView ndsv) ndsv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is PasswordChangeView npcv) npcv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is LanguageSelectionView nlsv) nlsv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is TalonView ntv) ntv.NavigationRequested += OnNavigationRequested; // Subscribe TalonView
            if (_currentContentView is BankView nbv) nbv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is ChangePhotoView ncpv) ncpv.NavigationRequested += OnNavigationRequested;
            if (_currentContentView is ChangeAdminNameView ncanv) ncanv.NavigationRequested += OnNavigationRequested;

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

        private void OpenBank_Click(object? sender, RoutedEventArgs e) => SetMainContent(new TalonView());
        private void OpenTalon_Click(object? sender, RoutedEventArgs e) => SetMainContent(new BankView());
        private void OpenDashboard_Click(object? sender, RoutedEventArgs e) => SetMainContent(new DashboardView());
        private void OpenSettings_Click(object? sender, RoutedEventArgs e) => SetMainContent(new SettingsView());
        private void ExitApp_Click(object? sender, RoutedEventArgs e) => this.Close();
    }
}
