using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.DB.Repositories;
using QManager.Service;
using System;

namespace QManager.ViewModels
{
    public partial class ChangeAdminNameViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _newUsername = string.Empty;

        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isError;

        private readonly AdminRepository _adminRepository;
        private string _currentUsername;

        public ChangeAdminNameViewModel()
        {
            _adminRepository = new AdminRepository();
            _currentUsername = SessionState.Username;
            NewUsername = _currentUsername;
        }

        [RelayCommand]
        private void ApplyNameChange()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(CurrentPassword))
            {
                StatusMessage = LocalizationService.Instance["RequiredFields"];
                IsError = true;
                return;
            }

            bool success = _adminRepository.UpdateUsername(_currentUsername, NewUsername.Trim(), CurrentPassword);

            if (success)
            {
                SessionState.UpdateUsername(NewUsername.Trim());
                _currentUsername = NewUsername.Trim();
                StatusMessage = LocalizationService.Instance["NameUpdated"];
                CurrentPassword = string.Empty;
                IsError = false;
            }
            else
            {
                StatusMessage = LocalizationService.Instance["WrongCredentials"];
                IsError = true;
            }
        }
    }
}