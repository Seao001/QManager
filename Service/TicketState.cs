using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QManager.Service
{
    public sealed class TicketState : INotifyPropertyChanged
    {
        public static TicketState Instance { get; } = new TicketState();

        public ObservableCollection<TicketViewModel> OnHoldTickets { get; } = new();
        public ObservableCollection<TicketViewModel> ReadyTickets { get; } = new();

        public string WaitingNowText => $"Waiting Now - {OnHoldTickets.Count}";

        public event PropertyChangedEventHandler? PropertyChanged;

        private TicketState()
        {
            OnHoldTickets.CollectionChanged += OnCollectionChanged;
            ReadyTickets.CollectionChanged += OnCollectionChanged;
            InitializeDemoData();
        }

        public void AddTicket(string talon, string room)
        {
            if (string.IsNullOrWhiteSpace(talon) || string.IsNullOrWhiteSpace(room))
            {
                return;
            }

            OnHoldTickets.Add(new TicketViewModel(talon, room));
            OnPropertyChanged(nameof(WaitingNowText));
        }

        public void MoveToReady(TicketViewModel ticket)
        {
            if (ticket is null)
            {
                return;
            }

            if (OnHoldTickets.Remove(ticket))
            {
                ReadyTickets.Add(ticket);
            }
        }

        public void MoveToOnHold(TicketViewModel ticket)
        {
            if (ticket is null)
            {
                return;
            }

            if (ReadyTickets.Remove(ticket))
            {
                OnHoldTickets.Add(ticket);
            }
        }

        private void InitializeDemoData()
        {
            OnHoldTickets.Add(new TicketViewModel("A11", "03"));
            OnHoldTickets.Add(new TicketViewModel("D21", "03"));
            OnHoldTickets.Add(new TicketViewModel("A12", "03"));
            OnHoldTickets.Add(new TicketViewModel("H44", "03"));
            OnHoldTickets.Add(new TicketViewModel("F13", "011"));
            OnHoldTickets.Add(new TicketViewModel("G12", "011"));
            OnHoldTickets.Add(new TicketViewModel("Y12", "011"));

            ReadyTickets.Add(new TicketViewModel("A13", "03"));
            ReadyTickets.Add(new TicketViewModel("E23", "03"));
            OnPropertyChanged(nameof(WaitingNowText));
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(WaitingNowText));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class TicketViewModel
    {
        public TicketViewModel(string talon, string room)
        {
            Talon = talon;
            Room = room;
        }

        public string Talon { get; }
        public string Room { get; }
        public string RoomLabel => $"Room {Room}";
    }
}
