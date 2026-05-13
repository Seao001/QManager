using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QManager.Service;

namespace QManager.View
{
    public partial class TalonView : UserControl, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;
        private readonly TicketState _ticketState = TicketState.Instance;

        public ObservableCollection<TicketViewModel> OnHoldTickets => _ticketState.OnHoldTickets;
        public ObservableCollection<TicketViewModel> ReadyTickets => _ticketState.ReadyTickets;

        public TalonView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
