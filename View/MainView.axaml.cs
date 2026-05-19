using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class MainView : UserControl
    {
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public MainView()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}