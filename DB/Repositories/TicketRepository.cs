using MySql.Data.MySqlClient;
using QManager.Models;

namespace QManager.DB.Repositories
{
    public class TicketRepository
    {
        private readonly DbConnection _db = new DbConnection();

        public void AddTicket(Ticket ticket)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                string sql = "INSERT INTO Tickets (ticket_label, service_id, status, created_at) VALUES (@label, @sId, 'On hold', NOW())";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@label", ticket.TicketLabel);
                cmd.Parameters.AddWithValue("@sId", ticket.ServiceId);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
            }
        }
    }
}
