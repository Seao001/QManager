using System.Collections.Generic;

namespace QManager.Models
{
    // Clasă care grupează toate tichetele active pentru a fi salvate într-un singur fișier JSON
    public class DashboardState
    {
        public List<TicketStateDto> Tickets { get; set; } = new();
    }
}