using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.Service;
using System.Collections.Generic;
using System.Linq;

namespace QManager.ViewModels
{
    public partial class LanguageSelectionViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> _languages = new() { "Română", "English" };

        [ObservableProperty]
        private string _selectedLanguage;

        public LanguageSelectionViewModel()
        {
            _selectedLanguage = LocalizationService.Instance.CurrentLanguage == AppLanguage.Romanian 
                ? "Română" 
                : "English";
        }

        [RelayCommand]
        private void ApplyLanguage()
        {
            var language = SelectedLanguage == "Română" 
                ? AppLanguage.Romanian 
                : AppLanguage.English;

            LocalizationService.Instance.SetLanguage(language);
            
            // Notă: Pentru ca toate textele să se actualizeze instantaneu, 
            // ar fi ideal ca restul View-urilor să fie reîncărcate sau să folosească Bindings dinamice.
        }
    }
}