using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QManager.Service;
using Avalonia.Threading; // Adăugat pentru DispatcherTimer

namespace QManager.View
{
    public partial class TalonView : UserControl
    {
        private TextBlock _realTimeClock = null!; // Adăugat pentru ceas

        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;
        private readonly TicketState _ticketState = TicketState.Instance;

        public ObservableCollection<TicketViewModel> OnHoldTickets => _ticketState.OnHoldTickets;
        public ObservableCollection<TicketViewModel> ReadyTickets => _ticketState.ReadyTickets;

        public TalonView()
        {
            AvaloniaXamlLoader.Load(this);
            ResolveControls(); // Adăugat pentru a găsi controalele
            InitializeRealTimeClock(); // Adăugat pentru a porni ceasul
            DataContext = this;
        }

        private void ResolveControls()
        {
            _realTimeClock = GetRequiredControl<TextBlock>("RealTimeClock");
        }

        // Metodă helper pentru a găsi controale, preluată din alte View-uri
        private T GetRequiredControl<T>(string name) where T : Control =>
            this.FindControl<T>(name) ?? throw new InvalidOperationException($"{typeof(T).Name} '{name}' was not found in the XAML.");

        private void InitializeRealTimeClock()
        {
            // Setăm ora inițială imediat
            _realTimeClock.Text = DateTime.Now.ToString("HH:mm:ss");

            // Creăm un timer care rulează la fiecare secundă
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += (s, e) =>
            {
                _realTimeClock.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            timer.Start();
        }
    }
}
