using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using QManager.Service;

namespace QManager.View
{
    public partial class DashboardView : UserControl, INotifyPropertyChanged
    {
        private string _averageWaitText = "Avg. Wait Time - 05:20 min";
        private readonly TicketState _ticketState = TicketState.Instance;

        public new event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public ObservableCollection<TicketViewModel> OnHoldTickets => _ticketState.OnHoldTickets;
        public ObservableCollection<TicketViewModel> ReadyTickets => _ticketState.ReadyTickets;

        public string WaitingNowText => _ticketState.WaitingNowText;

        public string AverageWaitText
        {
            get => _averageWaitText;
            private set
            {
                if (_averageWaitText == value)
                {
                    return;
                }

                _averageWaitText = value;
                OnPropertyChanged();
            }
        }

        public DashboardView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            _ticketState.PropertyChanged += OnTicketStateChanged;
        }

        private void OnTicketStateChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TicketState.WaitingNowText))
            {
                OnPropertyChanged(nameof(WaitingNowText));
            }
        }

        private void MoveToReady_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: TicketViewModel ticket })
            {
                return;
            }

            _ticketState.MoveToReady(ticket);
        }

        private void ResetQueue_Click(object? sender, RoutedEventArgs e)
        {
            OnHoldTickets.Clear();
            ReadyTickets.Clear();
            AverageWaitText = "Avg. Wait Time - 00:00 min";
        }

        private async void ExportData_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null)
            {
                return;
            }

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export queue data",
                SuggestedFileName = $"queue-dashboard-{DateTime.Now:yyyy-MM-dd-HHmm}.csv",
                DefaultExtension = "csv",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("CSV file")
                    {
                        Patterns = new[] { "*.csv" },
                        MimeTypes = new[] { "text/csv" }
                    }
                }
            });

            if (file is null)
            {
                return;
            }

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream, Encoding.UTF8);

            await writer.WriteLineAsync("Status,Talon,Room");
            foreach (var ticket in OnHoldTickets)
            {
                await writer.WriteLineAsync($"On hold,{ticket.Talon},{ticket.Room}");
            }

            foreach (var ticket in ReadyTickets)
            {
                await writer.WriteLineAsync($"Ready for service,{ticket.Talon},{ticket.Room}");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
