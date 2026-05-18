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
    public partial class TalonView : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        private TextBlock _realTimeClock = null!; // Adăugat pentru ceas

        private readonly TicketState _ticketState = TicketState.Instance;

        public string BankFilter { get; }

        public ObservableCollection<TicketViewModel> OnHoldTickets { get; } = new();
        public ObservableCollection<TicketViewModel> ReadyTickets { get; } = new();

        public string DisplayBankName => string.IsNullOrEmpty(BankFilter) 
            ? LocalizationService.Instance["MainTitle"] 
            : BankFilter;

        public TalonView()
            : this(string.Empty) // Constructor implicit, afișează toate băncile dacă nu e specificat un filtru
        {
        }

        public TalonView(string bankFilter)
        {
            BankFilter = bankFilter;
            AvaloniaXamlLoader.Load(this);
            ResolveControls(); // Adăugat pentru a găsi controalele
            _ticketState.OnHoldTickets.CollectionChanged += (s, e) => FilterTickets();
            _ticketState.ReadyTickets.CollectionChanged += (s, e) => FilterTickets();
            LocalizationService.Instance.LanguageChanged += (s, e) => 
            {
                FilterTickets();
                OnPropertyChanged(nameof(DisplayBankName));
            };
            FilterTickets(); // Populează listele inițial
            InitializeRealTimeClock();
            DataContext = this;
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void FilterTickets()
        {
            OnHoldTickets.Clear();
            ReadyTickets.Clear();

            foreach (var ticket in _ticketState.OnHoldTickets)
            {
                if (string.IsNullOrEmpty(BankFilter) || ticket.Bank == BankFilter)
                    OnHoldTickets.Add(ticket);
            }

            foreach (var ticket in _ticketState.ReadyTickets)
            {
                if (string.IsNullOrEmpty(BankFilter) || ticket.Bank == BankFilter)
                    ReadyTickets.Add(ticket);
            }
        }

        private void Back_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("Bank")); // Navigate back to BankView
        }
    }
}
