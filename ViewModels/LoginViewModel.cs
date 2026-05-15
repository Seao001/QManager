using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.DB.Repositories;
using QManager.Models;

namespace QManager.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AdminRepository _adminRepo = new AdminRepository();

        public event EventHandler? LoginSucceeded;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private Admin? _loggedInAdmin;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [RelayCommand]
        private void Login()
        {
            LoggedInAdmin = _adminRepo.Login(Username, Password);
            if (LoggedInAdmin != null)
            {
                ErrorMessage = string.Empty;
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = "Incorrect username or password.";
            }
        }
    }
}
