using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.DB.Repositories;
using QManager.Service;
using System;
using Avalonia.Media.Imaging;
using System.IO;

namespace QManager.ViewModels
{
    public partial class ChangePhotoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _selectedPhotoPath = string.Empty;

        [ObservableProperty]
        private Bitmap? _profileImage;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isError;

        private readonly AdminRepository _adminRepository;
        private readonly string _username;

        public ChangePhotoViewModel()
        {
            _adminRepository = new AdminRepository();
            _username = SessionState.Username;
            _selectedPhotoPath = SessionState.ProfilePhotoPath;
            LoadImage(_selectedPhotoPath);
        }

        partial void OnSelectedPhotoPathChanged(string value)
        {
            // Salvăm automat doar dacă este o cale nouă, diferită de cea din sesiune
            if (!string.IsNullOrEmpty(value) && value != SessionState.ProfilePhotoPath)
            {
                LoadImage(value);
            }
        }

        private void LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                ProfileImage = null;
        System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] ProfileImage set to null (path empty or file not found). Path: '{path}'");
                return;
            }

    try {
        ProfileImage = new Bitmap(path);
        System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] ProfileImage set to new Bitmap from: '{path}'");
    } catch (Exception ex) {
        ProfileImage = null;
        System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] Error creating Bitmap from '{path}': {ex.Message}");
    }
        }

        [RelayCommand]
        private void SavePhoto()
        {
            if (string.IsNullOrEmpty(SelectedPhotoPath)) return;

            string finalPath = SelectedPhotoPath;

            try
            {
                string extension = Path.GetExtension(SelectedPhotoPath);
                string fileName = $"profile_{_username}_{DateTime.Now.Ticks}{extension}";
                string destinationPath = Path.Combine(SessionState.SessionDirectory, fileName);

                if (SelectedPhotoPath != destinationPath && File.Exists(SelectedPhotoPath))
                {
                    Directory.CreateDirectory(SessionState.SessionDirectory);
                    File.Copy(SelectedPhotoPath, destinationPath, true);
                    finalPath = destinationPath;
                    System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] Copied photo from '{SelectedPhotoPath}' to '{destinationPath}'. Final path: '{finalPath}'");
                }
                else System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] Photo not copied. SelectedPath: '{SelectedPhotoPath}', Destination: '{destinationPath}', Exists: {File.Exists(SelectedPhotoPath)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] Storage Error during photo copy: {ex.Message}");
            }

            if (_adminRepository.UpdateProfilePhoto(_username, finalPath))
            {
                SessionState.UpdateProfilePhoto(finalPath);
                SelectedPhotoPath = finalPath; 
                StatusMessage = LocalizationService.Instance["PhotoUpdated"];
                IsError = false;
                System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] Photo updated successfully in DB and SessionState. New SelectedPhotoPath: '{SelectedPhotoPath}'");
            }
            else
            {
                StatusMessage = LocalizationService.Instance["CreateAccountFailed"]; // Reutilizăm pentru eroare generică
                IsError = true;
                System.Diagnostics.Debug.WriteLine($"[ChangePhotoViewModel] Failed to update photo in DB. Path: '{finalPath}'");
            }
        }
    }
}