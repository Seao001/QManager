using System;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using QManager.DB;
using QManager.Models;

namespace QManager.DB.Repositories
{
    public class AdminRepository
    {
        private readonly DbConnection _db = new DbConnection();

        public bool Register(string name, string password)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();

                var username = name.Trim();

                const string existsSql = "SELECT COUNT(*) FROM Admins WHERE username = @username";
                using (var existsCmd = new MySqlCommand(existsSql, conn))
                {
                    existsCmd.Parameters.AddWithValue("@username", username);
                    var exists = Convert.ToInt32(existsCmd.ExecuteScalar()) > 0;
                    if (exists) return false;
                }

                const string insertSql = @"
                    INSERT INTO Admins (username, password_hash, theme, language)
                    VALUES (@username, @password_hash, @theme, @language)";

                using var insertCmd = new MySqlCommand(insertSql, conn);
                insertCmd.Parameters.AddWithValue("@username", username);
                insertCmd.Parameters.AddWithValue("@password_hash", HashPassword(password));
                insertCmd.Parameters.AddWithValue("@theme", "Light");
                insertCmd.Parameters.AddWithValue("@language", "RO");

                insertCmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Admin? Login(string username, string password)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();

                const string sql = @"
                    SELECT admin_id, username, password_hash, theme, language, profile_photo, created_at
                    FROM Admins
                    WHERE username = @username
                    LIMIT 1";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username.Trim());

                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;

                var storedHash = reader.GetString("password_hash");
                if (!VerifyPassword(password, storedHash)) return null;

                return new Admin
                {
                    AdminId = reader.GetInt32("admin_id"),
                    Username = reader.GetString("username"),
                    PasswordHash = storedHash,
                    Theme = reader.GetString("theme") == "Dark" ? UserTheme.Dark : UserTheme.Light,
                    ProfilePhotoPath = reader.IsDBNull(reader.GetOrdinal("profile_photo")) ? string.Empty : reader.GetString("profile_photo"),
                    Language = reader.GetString("language"),
                    CreatedAt = reader.GetDateTime("created_at")
                };
            }
            catch
            {
                return null;
            }
        }

        public bool ChangePassword(string username, string currentPassword, string newPassword)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();

                const string selectSql = "SELECT password_hash FROM Admins WHERE username = @username LIMIT 1";
                string storedHash;
                using (var cmd = new MySqlCommand(selectSql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username.Trim());
                    var result = cmd.ExecuteScalar();
                    if (result == null) return false;
                    storedHash = result.ToString()!;
                }

                if (!VerifyPassword(currentPassword, storedHash)) return false;

                const string updateSql = "UPDATE Admins SET password_hash = @new_hash WHERE username = @username";
                using (var updateCmd = new MySqlCommand(updateSql, conn))
                {
                    updateCmd.Parameters.AddWithValue("@new_hash", HashPassword(newPassword));
                    updateCmd.Parameters.AddWithValue("@username", username.Trim());
                    return updateCmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUsername(string oldUsername, string newUsername, string password)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();

                // 1. Verificăm parola curentă
                const string selectSql = "SELECT password_hash FROM Admins WHERE username = @username LIMIT 1";
                string storedHash;
                using (var cmd = new MySqlCommand(selectSql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", oldUsername.Trim());
                    var result = cmd.ExecuteScalar();
                    if (result == null) return false;
                    storedHash = result.ToString()!;
                }

                if (!VerifyPassword(password, storedHash)) return false;

                // 2. Dacă parola e corectă, actualizăm numele
                const string updateSql = "UPDATE Admins SET username = @newUsername WHERE username = @oldUsername";
                using var updateCmd = new MySqlCommand(updateSql, conn);
                updateCmd.Parameters.AddWithValue("@newUsername", newUsername.Trim());
                updateCmd.Parameters.AddWithValue("@oldUsername", oldUsername.Trim());

                return updateCmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateProfilePhoto(string username, string photoPath)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();

                const string sql = "UPDATE Admins SET profile_photo = @photo WHERE username = @username";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@photo", photoPath);
                cmd.Parameters.AddWithValue("@username", username.Trim());

                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            const int iterations = 100_000;
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                32);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string storedValue)
        {
            try
            {
                var parts = storedValue.Split('.', 3);
                if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations)) return false;

                var salt = Convert.FromBase64String(parts[1]);
                var expectedHash = Convert.FromBase64String(parts[2]);
                var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
