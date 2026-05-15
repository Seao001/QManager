using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.DB.Repositories;
using QManager.Service;
using System;

namespace QManager.ViewModels
{
    public partial class PasswordChangeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isError;

        private readonly AdminRepository _adminRepository;
        private readonly string _username;

        public PasswordChangeViewModel()
        {
            _adminRepository = new AdminRepository();
            _username = SessionState.Username; // Preluăm numele de utilizator din sesiunea curentă
        }
        [RelayCommand]
        private void ApplyPasswordChange()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                StatusMessage = "Please fill in all fields.";
                IsError = true;
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                StatusMessage = "New passwords do not match.";
                IsError = true;
                return;
            }

            // Încercăm să schimbăm parola prin AdminRepository
            bool success = _adminRepository.ChangePassword(_username, CurrentPassword, NewPassword);

            if (success)
            {
                StatusMessage = "Password updated successfully!";
                IsError = false;
                CurrentPassword = string.Empty; // Resetăm câmpurile după succes
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
            }
            else
            {
                StatusMessage = "Failed to update password. Please check your current password.";
                IsError = true;
            }
        }
    }
}