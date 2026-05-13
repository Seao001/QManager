using System;
using System.IO;
using System.Text.Json;

namespace QManager.Service
{
    public static class SessionState
    {
        private static readonly TimeSpan DefaultLifetime = TimeSpan.FromDays(7);
        private static readonly string SessionDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QManager");
        private static readonly string SessionFilePath = Path.Combine(SessionDirectory, "session.json");

        public static bool IsSignedIn { get; private set; }
        public static string Username { get; private set; } = string.Empty;
        public static DateTimeOffset? ExpiresAtUtc { get; private set; }

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

        public static void SignIn(string username)
        {
            SignIn(username, DefaultLifetime);
        }

        public static void SignIn(string username, TimeSpan lifetime)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return;
            }

            IsSignedIn = true;
            Username = username.Trim();
            ExpiresAtUtc = DateTimeOffset.UtcNow.Add(lifetime);
            Save();
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
                ExpiresAtUtc = ExpiresAtUtc ?? DateTimeOffset.UtcNow.Add(DefaultLifetime)
            };

            File.WriteAllText(SessionFilePath, JsonSerializer.Serialize(payload));
        }

        private static void ClearRuntime()
        {
            IsSignedIn = false;
            Username = string.Empty;
            ExpiresAtUtc = null;
        }

        private sealed class SessionPayload
        {
            public string Username { get; set; } = string.Empty;
            public DateTimeOffset ExpiresAtUtc { get; set; }
        }
    }
}
