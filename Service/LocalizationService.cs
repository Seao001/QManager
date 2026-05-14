using System;
using System.Collections.Generic;

namespace QManager.Service
{
    public enum AppLanguage
    {
        Romanian,
        English
    }

    public sealed class LocalizationService
    {
        private static readonly LocalizationService _instance = new();

        private readonly Dictionary<AppLanguage, Dictionary<string, string>> _translations = new()
        {
            [AppLanguage.Romanian] = new Dictionary<string, string>
            {
                ["LoginTitle"] = "Autentificare - Queue Manager",
                ["Username"] = "Utilizator",
                ["Password"] = "Parola",
                ["Login"] = "Conectare",
                ["SignUp"] = "Sign Up",
                ["SignUpTitle"] = "Sign Up - Queue Manager",
                ["ConfirmPassword"] = "Confirmare parola",
                ["CreateAccount"] = "Creeaza cont",
                ["BackToSignIn"] = "Inapoi la autentificare",
                ["SuccessLoading"] = "Succes! Se incarca...",
                ["WrongCredentials"] = "Utilizator sau parola incorecta.",
                ["MainTitle"] = "Sistem Gestiune Cozi",
                ["SelectService"] = "Selectati serviciul dorit",
                ["OperatorPanel"] = "Panou Operator",
                ["CurrentClient"] = "Client curent:",
                ["NextClient"] = "URMATORUL CLIENT",
                ["SystemSettings"] = "Setari Sistem",
                ["ExitApp"] = "Ieșire"
            },
            [AppLanguage.English] = new Dictionary<string, string>
            {
                ["LoginTitle"] = "Login - Queue Manager",
                ["Username"] = "Username",
                ["Password"] = "Password",
                ["Login"] = "Login",
                ["SignUp"] = "Sign Up",
                ["SignUpTitle"] = "Sign Up - Queue Manager",
                ["ConfirmPassword"] = "Confirm Password",
                ["CreateAccount"] = "Create account",
                ["BackToSignIn"] = "Back to Sign In",
                ["SuccessLoading"] = "Success! Loading...",
                ["WrongCredentials"] = "Incorrect username or password.",
                ["MainTitle"] = "Queue Manager",
                ["SelectService"] = "Select the desired service",
                ["OperatorPanel"] = "Operator Panel",
                ["CurrentClient"] = "Current client:",
                ["NextClient"] = "NEXT CLIENT",
                ["SystemSettings"] = "System Settings",
                ["ExitApp"] = "Exit"
            }
        };

        private AppLanguage _currentLanguage = AppLanguage.English;

        public static LocalizationService Instance => _instance;

        public AppLanguage CurrentLanguage => _currentLanguage;

        public event EventHandler? LanguageChanged;

        public string this[string key] => _translations[_currentLanguage][key];

        public void SetLanguage(AppLanguage language)
        {
            if (_currentLanguage == language)
            {
                return;
            }

            _currentLanguage = language;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
