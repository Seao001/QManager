using Avalonia.Controls;
using System;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using QManager.DB.Repositories;
using QManager.Models;
using QManager.Service;

namespace QManager.View
{
    public partial class BankView : UserControl
    {
        private readonly TicketRepository _ticketRepository = new();
        private TextBox _talonTextBox = null!; // Păstrăm TalonTextBox
        private ComboBox _roomComboBox = null!;
        private TextBlock _statusTextBlock = null!;
        private string _bankName = string.Empty; // Numele băncii pentru această instanță

        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public BankView(string bankName)
        {
            _bankName = bankName;
            AvaloniaXamlLoader.Load(this);
            ResolveControls();
            InitializeDropdowns();
            DataContext = this; // Setează DataContext pentru a afișa numele băncii
        }

        private void ResolveControls()
        {
            _talonTextBox = GetRequiredControl<TextBox>("TalonTextBox");
            _roomComboBox = GetRequiredControl<ComboBox>("RoomComboBox"); // Păstrăm RoomComboBox
            _statusTextBlock = GetRequiredControl<TextBlock>("StatusTextBlock");
        }

        private void InitializeDropdowns()
        {
            _roomComboBox.ItemsSource = new List<string> 
            { 
                "011 - Credits", 
                "012 - Deposits", 
                "03 - General Service", 
                "05 - Insurance" 
            };
            _roomComboBox.SelectedIndex = 0; // Selectează prima opțiune implicit
        }

        private T GetRequiredControl<T>(string name) where T : Control =>
            this.FindControl<T>(name) ?? throw new InvalidOperationException($"{typeof(T).Name} '{name}' was not found in the XAML.");

        private void GenerateTalon_Click(object? sender, RoutedEventArgs e)
        {
            var prefix = _bankName.Length > 0 ? _bankName[0].ToString().ToUpper() : "A";
            _talonTextBox.Text = $"{prefix}{DateTime.Now:ss}";
            _statusTextBlock.Text = LocalizationService.Instance["TalonGenerated"];
        }

        private void AddTicket_Click(object? sender, RoutedEventArgs e)
        {
            var talon = _talonTextBox.Text?.Trim() ?? string.Empty;
            var selectedRoom = _roomComboBox.SelectedItem?.ToString() ?? string.Empty;

            // Extragem doar numărul camerei din formatul "011 - Credits"
            var roomNumber = selectedRoom.Contains('-') ? selectedRoom.Split('-')[0].Trim() : selectedRoom;
            var serviceDescription = selectedRoom.Contains('-') ? selectedRoom.Split('-')[1].Trim() : LocalizationService.Instance["GeneralService"]; // Extragem descrierea serviciului

            if (string.IsNullOrWhiteSpace(talon) || string.IsNullOrWhiteSpace(selectedRoom))
            {
                _statusTextBlock.Text = LocalizationService.Instance["CompleteFields"];
                return;
            }

            var ticket = new Ticket
            {
                TicketLabel = talon, // Talonul generat
                Bank = _bankName,    // Numele băncii este acum fix
                Room = roomNumber,   // Numărul biroului
                ServiceDescription = serviceDescription, // Adăugăm descrierea serviciului
                ServiceId = 0,       // Poate fi setat la un ID de serviciu real dacă este cazul
            };

            // Adăugarea în TicketState va gestiona acum și salvarea în baza de date
            TicketState.Instance.AddTicket(ticket.TicketLabel, ticket.Room, _bankName, serviceDescription);
            _statusTextBlock.Text = string.Format(LocalizationService.Instance["TalonAdded"], talon, roomNumber);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // Dacă s-a dat click pe fundal sau pe un element care nu este TextBox,
            // forțăm UserControl-ul să ia focusul, făcând TextBox-ul să piardă focusul (blur).
            if (e.Source is not TextBox && e.Source is not ComboBox)
            {
                this.Focus();
            }
        }

        private void Back_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("Bank"));
        }

        // Proprietate pentru a expune numele băncii către XAML
        public string BankName => _bankName;
    }
}
