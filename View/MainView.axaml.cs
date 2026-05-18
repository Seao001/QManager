using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class MainView : UserControl
    {
        // Re-adăugăm evenimentul pentru a fi compatibil cu logica de navigare din MainWindow.
        // Chiar dacă nu este folosit în interiorul MainView, MainWindow are nevoie de el pentru a compila.
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}