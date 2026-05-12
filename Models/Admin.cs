using System;

namespace QManager.Models
{
    public enum UserTheme { Light, Dark }

    public class Admin
    {
        public int AdminId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserTheme Theme { get; set; } = UserTheme.Light;
        public string Language { get; set; } = "RO";
        public DateTime CreatedAt { get; set; }
    }
}