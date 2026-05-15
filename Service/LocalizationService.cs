using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Avalonia.Threading;

namespace QManager.Service
{
    public enum AppLanguage
    {
        Romanian,
        English
    }

    public sealed class LocalizationService : INotifyPropertyChanged
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
                ["ExitApp"] = "Ieșire",
                ["LanguageSettings"] = "Setări Limbă",
                ["SelectLanguage"] = "Selectați limba aplicației",
                ["Apply"] = "Aplică",
                ["Back"] = "Înapoi",
                ["DisplaySettings"] = "Setări Afișaj",
                ["Resolution"] = "Rezoluție",
                ["FullscreenMode"] = "Mod Ecran Complet",
                ["ChangePassword"] = "Schimbă Parola",
                ["CurrentPassword"] = "Parola Curentă",
                ["NewPassword"] = "Parola Nouă",
                ["ConfirmNewPassword"] = "Confirmă Parola Nouă",
                ["ApplyChange"] = "Aplică Modificarea",
                ["AboutUs"] = "Despre noi",
                ["Display"] = "Afișaj",
                ["ChangeLanguage"] = "Schimbă Limba",
                ["SignOut"] = "Deconectare",
                ["DarkLightMode"] = "Mod Întunecat/Luminos",
                ["ChangeAdminName"] = "Schimbă Nume",
                ["ChangePhoto"] = "Schimbă Fotografia",
                ["Bank"] = "Bancă",
                ["Talon"] = "Casier",
                ["Room"] = "Birou",
                ["Dashboard"] = "Bord",
                ["WaitingNow"] = "În așteptare acum",
                ["InWaiting"] = "În așteptare",
                ["Ready"] = "Gata de service",
                ["AdminInitial"] = "A",
                ["RequiredFields"] = "Toate câmpurile sunt obligatorii.",
                ["PasswordsDoNotMatch"] = "Parolele nu se potrivesc.",
                ["AccountExists"] = "Contul există deja.",
                ["AccountCreated"] = "Cont creat cu succes.",
                ["TalonGenerated"] = "Talon generat.",
                ["CompleteFields"] = "Completați talonul și biroul înainte de a adăuga.",
                ["TalonAdded"] = "A fost adăugat talonul {0} pentru biroul {1}.",
                ["AdminName"] = "Administrator",
                ["Update"] = "Actualizează",
                ["Save"] = "Salvează",
                ["NameUpdated"] = "Numele a fost actualizat cu succes.",
                ["PhotoUpdated"] = "Fotografia de profil a fost salvată."
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
                ["ExitApp"] = "Exit",
                ["LanguageSettings"] = "Language Settings",
                ["SelectLanguage"] = "Select Application Language",
                ["Apply"] = "Apply",
                ["Back"] = "Back",
                ["DisplaySettings"] = "Display Settings",
                ["Resolution"] = "Resolution",
                ["FullscreenMode"] = "Fullscreen Mode",
                ["ChangePassword"] = "Change Password",
                ["CurrentPassword"] = "Current Password",
                ["NewPassword"] = "New Password",
                ["ConfirmNewPassword"] = "Confirm New Password",
                ["ApplyChange"] = "Apply Change",
                ["AboutUs"] = "About us",
                ["Display"] = "Display",
                ["ChangeLanguage"] = "Change Language",
                ["SignOut"] = "Sign Out",
                ["DarkLightMode"] = "Dark/Light Mode",
                ["ChangeAdminName"] = "Change Name",
                ["ChangePhoto"] = "Change Photo",
                ["Bank"] = "Bank",
                ["Talon"] = "Cashier",
                ["Room"] = "Room",
                ["Dashboard"] = "Dashboard",
                ["WaitingNow"] = "Waiting now",
                ["InWaiting"] = "On Hold",
                ["Ready"] = "Ready for service",
                ["AdminInitial"] = "A",
                ["RequiredFields"] = "All fields are required.",
                ["PasswordsDoNotMatch"] = "Passwords do not match.",
                ["AccountExists"] = "Account already exists.",
                ["AccountCreated"] = "Account created successfully.",
                ["TalonGenerated"] = "Talon generated.",
                ["CompleteFields"] = "Complete talon and room before adding.",
                ["TalonAdded"] = "Added talon {0} for room {1}.",
                ["AdminName"] = "Administrator",
                ["Update"] = "Update",
                ["Save"] = "Save",
                ["NameUpdated"] = "Name updated successfully.",
                ["PhotoUpdated"] = "Profile photo has been saved."
            }
        };

        private AppLanguage _currentLanguage;

        private LocalizationService()
        {
            SettingsManager.LoadSettings();
            _currentLanguage = SettingsManager.CurrentSettings.Language == "Română" ? AppLanguage.Romanian : AppLanguage.English;
        }

        public static LocalizationService Instance => _instance;

        public AppLanguage CurrentLanguage => _currentLanguage;

        public event EventHandler? LanguageChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public string this[string key] => GetText(key);

        public string GetText(string key)
        {
            if (_translations[_currentLanguage].TryGetValue(key, out var translation))
                return translation;
            return key;
        }

        public void SetLanguage(AppLanguage language)
        {
            if (_currentLanguage == language) 
                return;

            _currentLanguage = language;
            SettingsManager.CurrentSettings.Language = language == AppLanguage.Romanian ? "Română" : "English";
            SettingsManager.SaveSettings();

            LanguageChanged?.Invoke(this, EventArgs.Empty);

            // Executăm notificarea pe firul principal de UI pentru a garanta refresh-ul instantaneu
            Dispatcher.UIThread.Post(() =>
            {
                // string.Empty notifică schimbarea TUTUROR proprietăților (inclusiv indexerul)
                OnPropertyChanged(string.Empty);
                
                // Notificări specifice pentru indexer ("Item" este numele intern al indexerului în C#)
                // Aceasta forțează re-evaluarea Binding-urilor de tip {Binding [Key]}
                OnPropertyChanged("Item");
                OnPropertyChanged("Item[]");
            });
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
