using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new SettingsViewModel();
        }
    }
}
