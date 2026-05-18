using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using QManager.DB.Repositories;
using QManager.Models;

namespace QManager.Service
{
    public sealed class TicketState : INotifyPropertyChanged
    {
        public static TicketState Instance { get; } = new TicketState();
        
        private readonly TicketRepository _ticketRepository = new();
        private readonly DashboardStateService _stateService = new();
        private readonly TicketExportService _exportService = new(); // Instanțiem serviciul de export
        private bool _isLoading = false;

        public ObservableCollection<TicketViewModel> OnHoldTickets { get; } = new();
        public ObservableCollection<TicketViewModel> ReadyTickets { get; } = new();

        public string WaitingNowText => $"{LocalizationService.Instance["WaitingNow"]} - {OnHoldTickets.Count}";

        public event PropertyChangedEventHandler? PropertyChanged;

        private TicketState()
        {
            OnHoldTickets.CollectionChanged += OnCollectionChanged;
            ReadyTickets.CollectionChanged += OnCollectionChanged;
            LocalizationService.Instance.LanguageChanged += (s, e) => {
                OnPropertyChanged(nameof(WaitingNowText));
                foreach (var t in OnHoldTickets) t.RefreshLabels();
                foreach (var t in ReadyTickets) t.RefreshLabels();
            };

            // Încercăm întâi să încărcăm starea salvată local (JSON) pentru a păstra ordinea și consistența UI-ului
            if (!LoadState())
            {
                // Dacă nu există un fișier de stare local, încercăm să aducem datele din baza de date
                LoadTicketsFromDatabase();
                
                // Dacă și baza de date este goală (prima rulare), punem datele demo
                if (OnHoldTickets.Count == 0 && ReadyTickets.Count == 0)
                    InitializeDemoData();
            }

            OnPropertyChanged(nameof(WaitingNowText));
        }

        public void AddTicket(string talon, string room, string bank, string serviceDescription)
        {
            if (string.IsNullOrWhiteSpace(talon) || string.IsNullOrWhiteSpace(room) || string.IsNullOrWhiteSpace(bank))
            {
                return;
            }

            // Adăugăm tichetul în baza de date
            var newTicket = new Models.Ticket
            {
                TicketLabel = talon,
                Bank = bank,
                Room = room,
                ServiceDescription = serviceDescription, // Adăugăm descrierea serviciului
                Status = "On hold" 
            };
            newTicket.TicketId = _ticketRepository.AddTicket(newTicket);
            OnHoldTickets.Add(new TicketViewModel(newTicket.TicketId, talon, room, bank, serviceDescription));
            OnPropertyChanged(nameof(WaitingNowText));
            _exportService.ExportTicketToTxt(newTicket); // Exportăm bonul după adăugarea în DB
        }

        private void LoadTicketsFromDatabase()
        {
            try
            {
                _isLoading = true;
                OnHoldTickets.Clear();
                ReadyTickets.Clear();

                var onHoldDbTickets = _ticketRepository.GetTicketsByStatus("On hold");
                foreach (var dbTicket in onHoldDbTickets)
                {
                    OnHoldTickets.Add(new TicketViewModel(dbTicket.TicketId, dbTicket.TicketLabel, dbTicket.Room, dbTicket.Bank, dbTicket.ServiceDescription));
                }
                
                var readyDbTickets = _ticketRepository.GetTicketsByStatus("Ready");
                foreach (var dbTicket in readyDbTickets)
                {
                    ReadyTickets.Add(new TicketViewModel(dbTicket.TicketId, dbTicket.TicketLabel, dbTicket.Room, dbTicket.Bank, dbTicket.ServiceDescription));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tickets from database: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void MoveToReady(TicketViewModel ticket)
        {
            if (ticket is null)
            {
                return;
            }

            if (OnHoldTickets.Remove(ticket))
            {
                _ticketRepository.UpdateTicketStatus(ticket.TicketId, "Ready");
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
                _ticketRepository.UpdateTicketStatus(ticket.TicketId, "On hold");
                OnHoldTickets.Add(ticket);
            }
        }

        public void ClearAll()
        {
            // 1. Marcăm tichetele ca fiind finalizate în baza de date
            _ticketRepository.ClearAllActiveTickets();

            // 2. Golim colecțiile din memorie
            OnHoldTickets.Clear();
            ReadyTickets.Clear();
        }

        public void SaveState()
        {
            // Prevenim salvarea în timp ce încărcăm datele de pe disc
            if (_isLoading) return; 

            // Mapăm colecțiile curente către formatul de persistență DTO
            var ticketsToSave = OnHoldTickets.Select(t => new TicketStateDto 
            { 
                TicketId = t.TicketId, 
                TicketLabel = t.Talon, 
                BankName = t.Bank, 
                RoomNumber = t.Room, 
                ServiceDescription = t.ServiceDescription,
                Status = "On hold" 
            }).Concat(ReadyTickets.Select(t => new TicketStateDto 
            { 
                TicketId = t.TicketId, 
                TicketLabel = t.Talon, 
                BankName = t.Bank, 
                RoomNumber = t.Room, 
                ServiceDescription = t.ServiceDescription,
                Status = "Ready" 
            }));

            _stateService.SaveCurrentState(ticketsToSave);
        }

        private bool LoadState()
        {
            try
            {
                var state = _stateService.LoadLastState();
                if (state == null || state.Tickets == null) return false;
                
                _isLoading = true;
                OnHoldTickets.Clear();
                ReadyTickets.Clear();

                // Reconstruim tichetele în listele corespunzătoare bazat pe Status
                foreach (var t in state.Tickets) 
                {
                    var vm = new TicketViewModel(t.TicketId, t.TicketLabel, t.RoomNumber, t.BankName, t.ServiceDescription);
                    if (t.Status == "Ready")
                        ReadyTickets.Add(vm);
                    else
                        OnHoldTickets.Add(vm);
                }

                return true; // Returnăm true pentru a confirma că am încărcat starea (chiar dacă e o listă goală)
            }
            catch
            {
                return false;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void InitializeDemoData()
        {
            OnHoldTickets.Add(new TicketViewModel(0, "M11", "03", "Maib", "Credite"));
            OnHoldTickets.Add(new TicketViewModel(0, "V21", "03", "VictoriaBank", "Depozite"));
            OnHoldTickets.Add(new TicketViewModel(0, "B12", "03", "BNM", "Serviciu General"));
            OnHoldTickets.Add(new TicketViewModel(0, "M44", "03", "MICB", "Asigurari"));
            OnHoldTickets.Add(new TicketViewModel(0, "M13", "011", "Maib", "Credite"));
            OnHoldTickets.Add(new TicketViewModel(0, "V12", "011", "VictoriaBank", "Depozite"));
            OnHoldTickets.Add(new TicketViewModel(0, "B12", "011", "BNM", "Serviciu General"));

            ReadyTickets.Add(new TicketViewModel(0, "M13", "03", "Maib", "Credite"));
            ReadyTickets.Add(new TicketViewModel(0, "V23", "03", "VictoriaBank", "Depozite"));
            OnPropertyChanged(nameof(WaitingNowText));
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(WaitingNowText));
            SaveState();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class TicketData
        {
            public int TicketId { get; set; }
            public string Talon { get; set; } = string.Empty;
            public string Room { get; set; } = string.Empty;
            public string Bank { get; set; } = string.Empty;
        }

        private class TicketsStatePayload
        {
            public List<TicketData> OnHold { get; set; } = new();
            public List<TicketData> Ready { get; set; } = new();
        }
    }

    public sealed class TicketViewModel : INotifyPropertyChanged
    { // Adăugăm ServiceDescription în constructor
        public TicketViewModel(int ticketId, string talon, string room, string bank, string serviceDescription)
        {
            TicketId = ticketId;
            Talon = talon;
            Room = room;
            Bank = bank;
            ServiceDescription = serviceDescription;
        }

        public int TicketId { get; }
        public string Talon { get; }
        public string Room { get; }
        public string Bank { get; }
        public string ServiceDescription { get; }
        public string RoomLabel => $"{LocalizationService.Instance["Room"]} {Room}";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RefreshLabels()
        {
            OnPropertyChanged(nameof(RoomLabel));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
