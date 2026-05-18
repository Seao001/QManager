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
            this.SystemDecorations = SystemDecorations.None;
            this.WindowState = WindowState.Maximized;
            ResolveControls();
            AttachViewNavigation();

            this.DataContext = this;
            UpdateMenuUserText();
            UpdateMenuAvatar(); // Adăugat: Actualizează avatarul la pornire
            LocalizationService.Instance.LanguageChanged += (s, e) => 
            {
                UpdateMenuUserText();
                GenerateBankMenus(); // Regenerăm butoanele pentru a traduce "Casier"/"Talon"
            };
            SessionState.UsernameChanged += (s, e) => UpdateMenuUserText();
            SessionState.ProfilePhotoChanged += (s, e) => UpdateMenuAvatar(); // Adăugat: Abonează-te la schimbările fotografiei de profil
            
            GenerateBankMenus(); // Generează butoanele de meniu pentru bănci
            SetMainContent(new MainView());
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
            _rootBorder = GetRequiredControl<Border>("RootBorder");
            _headerBorder = GetRequiredControl<Border>("HeaderBorder");
            _contentGrid = GetRequiredControl<Grid>("ContentGrid");
            _sidebarBackground = GetRequiredControl<Border>("SidebarBackground");
            _sidebarButtonArea = GetRequiredControl<Grid>("SidebarButtonArea");
            _mainContentBorder = GetRequiredControl<Border>("MainContentBorder");

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
                
                // Forțează pierderea focusului (blur) de pe orice TextBox activ
                this.Focus();
            };
        }

        private void GenerateBankMenus()
        {
            var banks = new List<string> { "Maib", "VictoriaBank", "BNM", "MICB" }; // Lista de bănci

            // Asigurăm că aceste controale sunt rezolvate înainte de a le goli/adăuga copii
            if (_bankCashierMenu == null || _bankDisplayMenu == null)
            {
                // Aceasta nu ar trebui să se întâmple dacă ResolveControls() este apelată corect înainte
                // dar ca o măsură de siguranță, putem re-rezolva sau arunca o excepție.
                // Pentru moment, presupunem că ResolveControls() funcționează.
                // Dacă această eroare persistă, verificați MainWindow.axaml pentru x:Name="BankCashierMenu" și x:Name="BankDisplayMenu"
                return; 
            }

            _bankCashierMenu.Children.Clear();
            _bankDisplayMenu.Children.Clear();

            foreach (var bank in banks)
            {
                var cashierLabel = LocalizationService.Instance["CashierSuffix"];
                var displayLabel = LocalizationService.Instance["DisplaySuffix"];

                // Butoane pentru Casier
                var cashierButton = new Button
                {
                    Content = $"{bank} {cashierLabel}",
                    Classes = { "menu" },
                    FontSize = 14,
                    Margin = new Avalonia.Thickness(12, 0, 0, 2)
                };
                cashierButton.Click += (s, e) => OnNavigationRequested(s, new NavigationRequestEventArgs($"{bank} Cashier"));
                _bankCashierMenu.Children.Add(cashierButton);

                // Butoane pentru Ecranul Principal (Talon)
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

            // Adaugă un buton "Toate Taloanele" pentru TalonView fără filtru
            var allTicketsButton = new Button 
            { 
                Content = LocalizationService.Instance["AllTickets"], 
                Classes = { "menu" }, 
                FontSize = 14, 
                Margin = new Avalonia.Thickness(12, 0, 0, 2) 
            };
            allTicketsButton.Click += (s, e) => OnNavigationRequested(s, new NavigationRequestEventArgs("Bank")); // Navighează la TalonView fără filtru
            _bankDisplayMenu.Children.Add(allTicketsButton);
        }

        private void OnNavigationRequested(object? sender, NavigationRequestEventArgs e)
        {
            // Verificăm dacă este o navigare specifică unei bănci
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
            else // Navigare generală
            {
                switch (e.Target)
                {
                    case "Home":
                        SetMainContent(new MainView());
                        break;
                    case "Bank": // Aceasta ar putea fi o navigare către un ecran de selecție bancară generală, sau o valoare implicită
                        SetMainContent(new TalonView()); // Afișează toate tichetele
                        break;
                    // Cazul "Casier" și "Talon" generic nu mai este necesar, deoarece navigăm către instanțe specifice băncii
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
                if (_currentContentView is DashboardView dv) dv.NavigationRequested -= OnNavigationRequested;
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
            if (_currentContentView is DashboardView ndv) ndv.NavigationRequested += OnNavigationRequested;

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

        // Logica pentru comutarea Modului TV / Fullscreen
        private void ToggleTVMode_Click(object? sender, RoutedEventArgs e)
        {
            _isTVMode = !_isTVMode;

            if (_isTVMode)
            {
                // Trecem în FullScreen și ascundem elementele de UI
                this.WindowState = WindowState.FullScreen;
                _headerBorder.IsVisible = false;
                _sidebarBackground.IsVisible = false;
                _sidebarButtonArea.IsVisible = false;
                _contentGrid.ColumnDefinitions[0].Width = new GridLength(0);
                
                // Eliminăm marginile și colțurile rotunjite pentru a ocupa 100% din ecran
                _rootBorder.Margin = new Thickness(0);
                _rootBorder.CornerRadius = new CornerRadius(0);
                _rootBorder.BorderThickness = new Thickness(0);
                
                _mainContentBorder.Margin = new Thickness(0);
                _mainContentBorder.CornerRadius = new CornerRadius(0);
                _mainContentBorder.BorderThickness = new Thickness(0);

                HideMenu(); // Închidem meniul lateral (burger) dacă era deschis
            }
            else
            {
                // Revenim la modul fereastră normală (Maximizat)
                this.WindowState = WindowState.Maximized;
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
