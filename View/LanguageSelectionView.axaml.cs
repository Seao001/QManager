using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class LanguageSelectionView : UserControl
    {
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public LanguageSelectionView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new LanguageSelectionViewModel();
        }

        private void Back_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("Settings"));
        }
    }
}