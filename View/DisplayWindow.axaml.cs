using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class DisplayWindow : Window
    {
        public DisplayWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new DisplayViewModel();
        }
    }
}