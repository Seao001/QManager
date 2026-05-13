using Avalonia.Controls;
using System;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QManager.DB.Repositories;
using QManager.Models;
using QManager.Service;

namespace QManager.View
{
    public partial class BankView : UserControl
    {
        private readonly TicketRepository _ticketRepository = new();
        private TextBox _talonTextBox = null!;
        private TextBox _roomTextBox = null!;
        private TextBlock _statusTextBlock = null!;

        public BankView()
        {
            AvaloniaXamlLoader.Load(this);
            ResolveControls();
        }

        private void ResolveControls()
        {
            _talonTextBox = GetRequiredControl<TextBox>("TalonTextBox");
            _roomTextBox = GetRequiredControl<TextBox>("RoomTextBox");
            _statusTextBlock = GetRequiredControl<TextBlock>("StatusTextBlock");
        }

        private T GetRequiredControl<T>(string name) where T : Control =>
            this.FindControl<T>(name) ?? throw new InvalidOperationException($"{typeof(T).Name} '{name}' was not found in the XAML.");

        private void GenerateTalon_Click(object? sender, RoutedEventArgs e)
        {
            _talonTextBox.Text = $"A{DateTime.Now:ss}";
            _statusTextBlock.Text = "Talon generated.";
        }

        private void AddTicket_Click(object? sender, RoutedEventArgs e)
        {
            var talon = _talonTextBox.Text?.Trim() ?? string.Empty;
            var room = _roomTextBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(talon) || string.IsNullOrWhiteSpace(room))
            {
                _statusTextBlock.Text = "Complete talon and room before adding.";
                return;
            }

            var ticket = new Ticket
            {
                TicketLabel = talon,
                ServiceId = 0
            };

            _ticketRepository.AddTicket(ticket);
            TicketState.Instance.AddTicket(talon, room);
            _statusTextBlock.Text = $"Added talon {talon} for room {room}.";
        }
    }
}
