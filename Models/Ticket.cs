using System;

namespace QManager.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string TicketLabel { get; set; } = string.Empty;
        public int ServiceId { get; set; } // Poate fi folosit pentru a lega de un serviciu mai detaliat
        public string Bank { get; set; } = string.Empty; // Numele băncii
        public string ServiceDescription { get; set; } = string.Empty; // Descrierea serviciului
        public string Room { get; set; } = string.Empty; // Numărul biroului
        public string Status { get; set; } = "On hold"; // Statusul tichetului (On hold, Ready, Completed)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}