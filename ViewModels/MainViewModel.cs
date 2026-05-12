using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QManager.DB.Repositories;
using QManager.Models;
using QManager.View;

namespace QManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly TicketRepository _ticketRepo = new TicketRepository();
        private readonly ServiceRepository _serviceRepo = new ServiceRepository();
        private Ticket? _currentTicket;

        // ObservableCollection de QueueService
        public ObservableCollection<QueueService> Services { get; } = new();

        public Ticket? CurrentTicket
        {
            get => _currentTicket;
            set => SetProperty(ref _currentTicket, value);
        }

        public ICommand GenerateTicketCommand { get; }
        public ICommand CallNextCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public MainViewModel()
        {
            GenerateTicketCommand = new RelayCommand<QueueService?>(GenerateTicket);
            CallNextCommand = new RelayCommand(CallNext);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            LoadServices();
        }

        private void LoadServices()
        {
            Services.Clear();
            var list = _serviceRepo.GetAllServices();
            foreach (var s in list) Services.Add(s);
        }

        private void GenerateTicket(QueueService? selectedService)
        {
            if (selectedService == null) return;
            
            var newTicket = new Ticket 
            { 
                TicketLabel = $"{selectedService.ServiceName}-001", // Logica de incrementare aici
                ServiceId = selectedService.ServiceId 
            };
            _ticketRepo.AddTicket(newTicket);
        }

        private void CallNext()
        {
            // TODO: connect this to the next waiting ticket from the database.
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }
    }
}
