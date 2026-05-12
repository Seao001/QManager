using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new LoginViewModel();
        }
    }
}
