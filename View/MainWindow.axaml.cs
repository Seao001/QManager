using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using System.Collections.Generic;
using Avalonia.Threading;
using QManager.Service;

namespace QManager.View
{
    public partial class MainWindow : Window
    {
        private Border _rootBorder = null!;
        private Border _headerBorder = null!;
        private Grid _contentGrid = null!;
        private Border _sidebarBackground = null!;
        private Grid _sidebarButtonArea = null!;
        private Border _mainContentBorder = null!;
        private Button _tvModeButton = null!;

        private Border _menuPanel = null!;
        private Button _menuBackdrop = null!;
        private TextBlock _menuUserText = null!;
        private Image _menuAvatarImage = null!; // Adăugat: Controlul Image pentru avatarul din meniu
        private TransitioningContentControl _mainContentPresenter = null!;
        private StackPanel _bankCashierMenu = null!; // Adăugat pentru meniul dinamic al casierilor
        private StackPanel _bankDisplayMenu = null!; // Adăugat pentru meniul dinamic al ecranelor principale
        private UserControl? _currentContentView;
        private bool _isTVMode = false;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            ResolveControls();

            this.DataContext = this;
            UpdateMenuUserText();
            UpdateMenuAvatar(); // Adăugat: Actualizează avatarul la pornire
            LocalizationService.Instance.LanguageChanged += (s, e) => 
            {
                UpdateMenuUserText();
                GenerateBankMenus();
            };
            SessionState.UsernameChanged += (s, e) => UpdateMenuUserText();
            SessionState.ProfilePhotoChanged += (s, e) => UpdateMenuAvatar();
            
            GenerateBankMenus();
            SetMainContent(new MainView());
            HideMenu();
            AttachViewNavigation(); // Atașăm navigarea după ce SetMainContent a fost apelat
            ApplyInitialState(); // Aplicăm starea inițială (Fullscreen)
        }

        private void UpdateMenuUserText()
        {
            _menuUserText.Text = string.IsNullOrWhiteSpace(SessionState.Username)
                ? LocalizationService.Instance["AdminName"]
                : SessionState.Username;
        }
        
        private void ApplyInitialState()
        {
            this.WindowState = WindowState.Maximized;
            UpdateLayoutForTVMode(false);
        }

        private void UpdateMenuAvatar()
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
            _rootBorder = GetRequiredControl<Border>("RootBorder");
            _headerBorder = GetRequiredControl<Border>("HeaderBorder");
            _contentGrid = GetRequiredControl<Grid>("ContentGrid");
            _sidebarBackground = GetRequiredControl<Border>("SidebarBackground");
            _sidebarButtonArea = GetRequiredControl<Grid>("SidebarButtonArea");
            _mainContentBorder = GetRequiredControl<Border>("MainContentBorder");
            _tvModeButton = GetRequiredControl<Button>("TVModeButton");

            _menuPanel = GetRequiredControl<Border>("MenuPanel");
            _menuBackdrop = GetRequiredControl<Button>("MenuBackdrop");
            _mainContentPresenter = GetRequiredControl<TransitioningContentControl>("MainContentPresenter");
            _menuUserText = GetRequiredControl<TextBlock>("MenuUserText");
            _menuAvatarImage = GetRequiredControl<Image>("MenuAvatarImage"); // Adăugat: Rezolvă noul control Image
            _bankCashierMenu = GetRequiredControl<StackPanel>("BankCashierMenu");
            _bankDisplayMenu = GetRequiredControl<StackPanel>("BankDisplayMenu");
        }

        private T GetRequiredControl<T>(string name) where T : Control =>
            this.FindControl<T>(name) ?? throw new InvalidOperationException($"{typeof(T).Name} '{name}' was not found in MainWindow.axaml.");

        private void AttachViewNavigation()
        {
            _menuBackdrop.Click += HideMenu_Click;

            _mainContentPresenter.PointerPressed += (s, e) => 
            { 
                if (_menuPanel.IsVisible) HideMenu(); 
                this.Focus();
            };
        }

        private void GenerateBankMenus()
        {
            var banks = new List<string> { "Maib", "VictoriaBank", "BNM", "MICB" }; // Lista de bănci

            // Asigurăm că aceste controale sunt rezolvate înainte de a le goli/adăuga copii
            if (_bankCashierMenu == null || _bankDisplayMenu == null)
            {
                return; 
            }

            _bankCashierMenu.Children.Clear();
            _bankDisplayMenu.Children.Clear();

            foreach (var bank in banks)
            {
                var cashierLabel = LocalizationService.Instance["CashierSuffix"];
                var displayLabel = LocalizationService.Instance["DisplaySuffix"];

                var cashierButton = new Button
                {
                    Content = $"{bank} {cashierLabel}",
                    Classes = { "menu" },
                    FontSize = 14,
                    Margin = new Avalonia.Thickness(12, 0, 0, 2)
                };
                cashierButton.Click += (s, e) => OnNavigationRequested(s, new NavigationRequestEventArgs($"{bank} Cashier"));
                _bankCashierMenu.Children.Add(cashierButton);

                var displayButton = new Button
                {
                    Content = $"{bank} {displayLabel}",
                    Classes = { "menu" },
                    FontSize = 14,
                    Margin = new Avalonia.Thickness(12, 0, 0, 2)
                };
                displayButton.Click += (s, e) => OnNavigationRequested(s, new NavigationRequestEventArgs($"{bank} Display"));
                _bankDisplayMenu.Children.Add(displayButton);
            }

            var allTicketsButton = new Button 
            { 
                Content = LocalizationService.Instance["AllTickets"], 
                Classes = { "menu" }, 
                FontSize = 14, 
                Margin = new Avalonia.Thickness(12, 0, 0, 2) 
            };
            allTicketsButton.Click += (s, e) => OnNavigationRequested(s, new NavigationRequestEventArgs("Bank"));
            _bankDisplayMenu.Children.Add(allTicketsButton);
        }

        private void OnNavigationRequested(object? sender, NavigationRequestEventArgs e)
        {
            if (e.Target.EndsWith("Cashier"))
            {
                var bankName = e.Target.Replace("Cashier", "").Trim();
                SetMainContent(new BankView(bankName));
            }
            else if (e.Target.EndsWith("Display"))
            {
                var bankName = e.Target.Replace("Display", "").Trim();
                SetMainContent(new TalonView(bankName));
            }
            else
            {
                switch (e.Target)
                {
                    case "Home":
                        SetMainContent(new MainView());
                        break;
                    case "Bank":
                        SetMainContent(new TalonView());
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
        }

        private void SetMainContent(UserControl view)
        {            
            ManageNavigationEvent(_currentContentView, false); // Dezabonăm vechiul view

            _currentContentView = view;
            ManageNavigationEvent(_currentContentView, true); // Abonăm noul view

            _mainContentPresenter.Content = view;
            HideMenu();
        }

        private void ManageNavigationEvent(UserControl? view, bool subscribe)
        {
            if (view is null) return;

            switch (view)
            {
                case MainView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case SettingsView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case PasswordChangeView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case LanguageSelectionView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case TalonView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case BankView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case ChangePhotoView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case ChangeAdminNameView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
                case DashboardView v: if (subscribe) v.NavigationRequested += OnNavigationRequested; else v.NavigationRequested -= OnNavigationRequested; break;
            }
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
            e.Handled = true;
        }

        private void ToggleTVMode_Click(object? sender, RoutedEventArgs e) => SetTVMode(!_isTVMode);

        private void SetTVMode(bool isTV)
        {
            this.WindowState = isTV ? WindowState.FullScreen : WindowState.Maximized;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == WindowStateProperty && _headerBorder != null)
            {
                _isTVMode = this.WindowState == WindowState.FullScreen;
                UpdateLayoutForTVMode(_isTVMode);
                if (_isTVMode) HideMenu(); // Închidem meniul lateral (burger) dacă era deschis
            }
        }

        private void UpdateLayoutForTVMode(bool isTV)
        {
            if (isTV)
            {
                _headerBorder.IsVisible = false;
                _sidebarBackground.IsVisible = false;
                _sidebarButtonArea.IsVisible = false;
                _contentGrid.ColumnDefinitions[0].Width = new GridLength(0);
                _rootBorder.Margin = new Thickness(0);
                _rootBorder.CornerRadius = new CornerRadius(0);
                _rootBorder.BorderThickness = new Thickness(0);
                _mainContentBorder.Margin = new Thickness(0);
                _mainContentBorder.CornerRadius = new CornerRadius(0);
                _mainContentBorder.BorderThickness = new Thickness(0);
            }
            else
            {
                _headerBorder.IsVisible = true;
                _sidebarBackground.IsVisible = true;
                _sidebarButtonArea.IsVisible = true;
                _contentGrid.ColumnDefinitions[0].Width = new GridLength(96);

                _rootBorder.Margin = new Thickness(10);
                _rootBorder.CornerRadius = new CornerRadius(16);
                _rootBorder.BorderThickness = new Thickness(2);

                _mainContentBorder.Margin = new Thickness(26);
                _mainContentBorder.CornerRadius = new CornerRadius(10);
                _mainContentBorder.BorderThickness = new Thickness(2);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Ieșire automată din modul TV la apăsarea tastei ESC
            if (e.Key == Key.Escape && _isTVMode)
            {
                ToggleTVMode_Click(null, new RoutedEventArgs());
            }
        }

        // Re-adăugăm metodele cerute de MainWindow.axaml pentru a fixa eroarea AVLN:0004
        private void OpenHome_Click(object? sender, RoutedEventArgs e) => OnNavigationRequested(this, new NavigationRequestEventArgs("Home"));

        private void OpenDashboard_Click(object? sender, RoutedEventArgs e) => OnNavigationRequested(this, new NavigationRequestEventArgs("Dashboard"));
        
        private void OpenSettings_Click(object? sender, RoutedEventArgs e) => OnNavigationRequested(this, new NavigationRequestEventArgs("Settings"));
        
        private void ExitApp_Click(object? sender, RoutedEventArgs e) => this.Close();
    }
}
