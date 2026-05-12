using System;

namespace QManager.Models
{
    public enum TicketStatus { OnHold, ReadyForService, Finished }

    public class Ticket
    {
        public int TicketId { get; set; }
        public string TicketLabel { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public int? CounterId { get; set; }
        public int? AdminId { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.OnHold;
        public DateTime CreatedAt { get; set; }
        public DateTime? CalledAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        
        // Pentru afișare rapidă în UI
        public string ServiceName { get; set; } = string.Empty;
    }
}