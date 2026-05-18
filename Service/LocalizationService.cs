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
                ["Home"] = "Acasă",
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
                ["BankCashierHeader"] = "Casieri Bănci",
                ["BankDisplayHeader"] = "Ecrane Principale",
                ["AllTickets"] = "Toate Taloanele",
                ["CashierSuffix"] = "Casier",
                ["DisplaySuffix"] = "Talon",
                ["InWaitingHeader"] = "În așteptare",
                ["ReadyHeader"] = "Gata de service",
                ["ResetQueue"] = "Resetează rândul",
                ["ExportData"] = "Exportă date",
                ["BankFilterLabel"] = "Filtru Bancă:",
                ["ReportFileName"] = "Raport-Coada",
                ["ReportTitle"] = "Raport Coadă de Așteptare",
                ["RememberMe"] = "Ține-mă minte",
                ["RequiredFieldsError"] = "Vă rugăm să completați toate câmpurile.",
                ["PasswordsDoNotMatchError"] = "Parolele noi nu se potrivesc.",
                ["PasswordUpdateSuccess"] = "Parola a fost actualizată cu succes!",
                ["Bank"] = "Bancă",
                ["ReceiptBankLabel"] = "Banca:",
                ["ReceiptServiceLabel"] = "Serviciu:",
                ["ReceiptTalonLabel"] = "TALON:",
                ["ReceiptDateLabel"] = "Data:",
                ["ReceiptTimeLabel"] = "Ora:",
                ["ReceiptFooterLine1"] = "Va rugam sa asteptati in zona de acord.",
                ["ReceiptFooterLine2"] = "Monitorizati ecranul din sala.",
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
                ["CreateAccountFailed"] = "Crearea contului a eșuat",
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
                ["Home"] = "Home",
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
                ["BankCashierHeader"] = "Bank Cashiers",
                ["BankDisplayHeader"] = "Main Displays",
                ["AllTickets"] = "All Tickets",
                ["CashierSuffix"] = "Cashier",
                ["DisplaySuffix"] = "Display",
                ["InWaitingHeader"] = "On Hold",
                ["ReadyHeader"] = "Ready for service",
                ["ResetQueue"] = "Reset Queue",
                ["ExportData"] = "Export data",
                ["BankFilterLabel"] = "Bank Filter:",
                ["ReportFileName"] = "Queue-Report",
                ["ReportTitle"] = "Queue Report",
                ["RememberMe"] = "Remember Me",
                ["RequiredFieldsError"] = "Please fill in all fields.",
                ["PasswordsDoNotMatchError"] = "New passwords do not match.",
                ["PasswordUpdateSuccess"] = "Password updated successfully!",
                ["Bank"] = "Bank",
                ["ReceiptBankLabel"] = "Bank:",
                ["ReceiptServiceLabel"] = "Service:",
                ["ReceiptTalonLabel"] = "TICKET:",
                ["ReceiptDateLabel"] = "Date:",
                ["ReceiptTimeLabel"] = "Time:",
                ["ReceiptFooterLine1"] = "Please wait in the designated area.",
                ["ReceiptFooterLine2"] = "Monitor the screen in the hall.",
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
                ["CreateAccountFailed"] = "Account creation failed",
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
