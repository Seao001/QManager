using System;
using System.IO;
using System.Text.Json;

namespace QManager.Service
{
    public static class SessionState
    {
        private static readonly TimeSpan DefaultLifetime = TimeSpan.FromDays(7);
        public static readonly string SessionDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QManager");
        private static readonly string SessionFilePath = Path.Combine(SessionDirectory, "session.json");

        public static bool IsSignedIn { get; private set; }
        public static string Username { get; private set; } = string.Empty;
        public static string ProfilePhotoPath { get; private set; } = string.Empty;
        public static DateTimeOffset? ExpiresAtUtc { get; private set; }

        public static event EventHandler? UsernameChanged;
        public static event EventHandler? ProfilePhotoChanged;

        public static bool Restore()
        {
            if (!File.Exists(SessionFilePath))
            {
                ClearRuntime();
                return false;
            }

            try
            {
                var json = File.ReadAllText(SessionFilePath);
                var payload = JsonSerializer.Deserialize<SessionPayload>(json);
                if (payload is null || string.IsNullOrWhiteSpace(payload.Username))
                {
                    SignOut();
                    return false;
                }

                var expiresAt = payload.ExpiresAtUtc;
                if (expiresAt <= DateTimeOffset.UtcNow)
                {
                    SignOut();
                    return false;
                }

                Username = payload.Username;
                ProfilePhotoPath = payload.ProfilePhotoPath;
                ExpiresAtUtc = expiresAt;
                IsSignedIn = true;
                return true;
            }
            catch
            {
                SignOut();
                return false;
            }
        }

        public static void SignIn(string username, string profilePhotoPath = "")
        {
            SignIn(username, DefaultLifetime, profilePhotoPath);
        }

        public static void SignIn(string username, TimeSpan lifetime, string profilePhotoPath = "")
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return;
            }

            IsSignedIn = true;
            Username = username.Trim();
            ProfilePhotoPath = profilePhotoPath;
            ExpiresAtUtc = DateTimeOffset.UtcNow.Add(lifetime);
            Save();
        }

        public static void UpdateUsername(string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername)) return;

            Username = newUsername.Trim();
            Save();
            UsernameChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void UpdateProfilePhoto(string newPath)
        {
            if (string.IsNullOrWhiteSpace(newPath)) return;

            ProfilePhotoPath = newPath;
            Save();
            ProfilePhotoChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SignOut()
        {
            ClearRuntime();

            try
            {
                if (File.Exists(SessionFilePath))
                {
                    File.Delete(SessionFilePath);
                }
            }
            catch
            {
            }
        }

        private static void Save()
        {
            Directory.CreateDirectory(SessionDirectory);

            var payload = new SessionPayload
            {
                Username = Username,
                ProfilePhotoPath = ProfilePhotoPath,
                ExpiresAtUtc = ExpiresAtUtc ?? DateTimeOffset.UtcNow.Add(DefaultLifetime)
            };

            File.WriteAllText(SessionFilePath, JsonSerializer.Serialize(payload));
        }

        private static void ClearRuntime()
        {
            IsSignedIn = false;
            Username = string.Empty;
            ProfilePhotoPath = string.Empty;
            ExpiresAtUtc = null;
        }

        private sealed class SessionPayload
        {
            public string Username { get; set; } = string.Empty;
            public string ProfilePhotoPath { get; set; } = string.Empty;
            public DateTimeOffset ExpiresAtUtc { get; set; }
        }
    }
}
