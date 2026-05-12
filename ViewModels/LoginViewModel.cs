using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.DB.Repositories;
using QManager.Models;

namespace QManager.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AdminRepository _adminRepo = new AdminRepository();

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [RelayCommand]
        private void Login()
        {
            var admin = _adminRepo.Login(Username, Password);
            if (admin != null)
            {
                // Aici vei adăuga logica pentru a deschide MainWindow
                ErrorMessage = "Succes! Se încarcă...";
            }
            else
            {
                ErrorMessage = "Utilizator sau parolă incorectă.";
            }
        }
    }
}
