using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using QManager.DB.Repositories;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel = new();
        private readonly AdminRepository _adminRepository = new();
        private Border _signInPanel = null!;
        private Border _signUpPanel = null!;
        private TextBox _signInUsernameTextBox = null!;
        private TextBox _signInPasswordTextBox = null!;
        private TextBox _nameTextBox = null!;
        private TextBox _passwordSignUpTextBox = null!;
        private TextBox _confirmPasswordTextBox = null!;
        private TextBlock _statusTextBlock = null!;

        public LoginWindow()
        {
            AvaloniaXamlLoader.Load(this);
            ResolveControls();

            _viewModel.LoginSucceeded += OnLoginSucceeded;
            DataContext = _viewModel;

            ShowSignInPanel();
        }

        private void ShowSignUpPanel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _statusTextBlock.Text = string.Empty;
            _nameTextBox.Text = string.Empty;
            _passwordSignUpTextBox.Text = string.Empty;
            _confirmPasswordTextBox.Text = string.Empty;

            _signInPanel.IsVisible = false;
            _signUpPanel.IsVisible = true;
            _nameTextBox.Focus();
        }

        private void ShowSignInPanel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            ShowSignInPanel();
        }

        private async void CreateAccount_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                var name = _nameTextBox.Text?.Trim() ?? string.Empty;
                var password = _passwordSignUpTextBox.Text ?? string.Empty;
                var confirmPassword = _confirmPasswordTextBox.Text ?? string.Empty;

                _statusTextBlock.Foreground = Brushes.OrangeRed;

                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    _statusTextBlock.Text = "Name, password and confirm password are required.";
                    return;
                }

                if (password != confirmPassword)
                {
                    _statusTextBlock.Text = "Passwords do not match.";
                    return;
                }

                var created = _adminRepository.Register(name, password);
                if (!created)
                {
                    _statusTextBlock.Text = "Account already exists or database is unavailable.";
                    return;
                }

                _statusTextBlock.Foreground = Brushes.Green;
                _statusTextBlock.Text = "Account created successfully.";
                await Task.Delay(900);

                _signUpPanel.IsVisible = false;
                _signInPanel.IsVisible = true;
                _viewModel.Username = name;
                _viewModel.Password = string.Empty;
                _viewModel.ErrorMessage = string.Empty;
                _signInUsernameTextBox.Text = name;
                _signInPasswordTextBox.Text = string.Empty;
                _signInUsernameTextBox.Focus();
            }
            catch (Exception ex)
            {
                _statusTextBlock.Foreground = Brushes.OrangeRed;
                _statusTextBlock.Text = $"Create account failed: {ex.Message}";
            }
        }

        private void OnLoginSucceeded(object? sender, EventArgs e)
        {
            var mainWindow = new MainWindow();
            if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = mainWindow;
            }

            mainWindow.Show();
            Close();
        }

        private void ShowSignInPanel()
        {
            _signUpPanel.IsVisible = false;
            _signInPanel.IsVisible = true;
            _viewModel.ErrorMessage = string.Empty;
            _signInUsernameTextBox.Text = _viewModel.Username;
            _signInPasswordTextBox.Text = string.Empty;
            _signInUsernameTextBox.Focus();
        }

        private void ResolveControls()
        {
            _signInPanel = this.FindControl<Border>("SignInPanel")
                ?? throw new InvalidOperationException("SignInPanel was not found in LoginWindow.axaml.");
            _signUpPanel = this.FindControl<Border>("SignUpPanel")
                ?? throw new InvalidOperationException("SignUpPanel was not found in LoginWindow.axaml.");
            _signInUsernameTextBox = this.FindControl<TextBox>("SignInUsernameTextBox")
                ?? throw new InvalidOperationException("SignInUsernameTextBox was not found in LoginWindow.axaml.");
            _signInPasswordTextBox = this.FindControl<TextBox>("SignInPasswordTextBox")
                ?? throw new InvalidOperationException("SignInPasswordTextBox was not found in LoginWindow.axaml.");
            _nameTextBox = this.FindControl<TextBox>("NameTextBox")
                ?? throw new InvalidOperationException("NameTextBox was not found in LoginWindow.axaml.");
            _passwordSignUpTextBox = this.FindControl<TextBox>("PasswordSignUpTextBox")
                ?? throw new InvalidOperationException("PasswordSignUpTextBox was not found in LoginWindow.axaml.");
            _confirmPasswordTextBox = this.FindControl<TextBox>("ConfirmPasswordTextBox")
                ?? throw new InvalidOperationException("ConfirmPasswordTextBox was not found in LoginWindow.axaml.");
            _statusTextBlock = this.FindControl<TextBlock>("StatusTextBlock")
                ?? throw new InvalidOperationException("StatusTextBlock was not found in LoginWindow.axaml.");
        }
    }
}
