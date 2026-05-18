using MySql.Data.MySqlClient;
using QManager.Models;
using System.Collections.Generic;
using System;

namespace QManager.DB.Repositories
{
    public class TicketRepository
    {
        private readonly DbConnection _db = new DbConnection();
        
        public int AddTicket(Ticket ticket)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                string sql = "INSERT INTO Tickets (ticket_label, service_id, bank_name, room_number, status, created_at, service_description) VALUES (@label, @sId, @bank, @room, @status, NOW(), @serviceDescription)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@label", ticket.TicketLabel);
                cmd.Parameters.AddWithValue("@sId", ticket.ServiceId);
                cmd.Parameters.AddWithValue("@bank", ticket.Bank);
                cmd.Parameters.AddWithValue("@room", ticket.Room);
                cmd.Parameters.AddWithValue("@status", ticket.Status);
                cmd.Parameters.AddWithValue("@serviceDescription", ticket.ServiceDescription);
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId; // Returnează ID-ul tichetului nou inserat
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error adding ticket: {ex.Message}");
                return -1;
            }
        }

        public List<Ticket> GetTicketsByStatus(string status)
        {
            var tickets = new List<Ticket>();
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                string sql = "SELECT ticket_id, ticket_label, service_id, bank_name, room_number, status, created_at, service_description FROM Tickets WHERE status = @status ORDER BY created_at ASC";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@status", status);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tickets.Add(new Ticket
                    {
                        TicketId = reader.GetInt32("ticket_id"),
                        TicketLabel = reader.GetString("ticket_label"),
                        ServiceId = reader.GetInt32("service_id"),
                        Bank = reader.GetString("bank_name"),
                        Room = reader.GetString("room_number"),
                        ServiceDescription = reader.GetString("service_description"),
                        Status = reader.GetString("status"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error getting tickets by status: {ex.Message}");
            }
            return tickets;
        }

        public bool UpdateTicketStatus(int ticketId, string newStatus)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                string sql = "UPDATE Tickets SET status = @newStatus WHERE ticket_id = @ticketId";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                cmd.Parameters.AddWithValue("@ticketId", ticketId);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error updating ticket status: {ex.Message}");
                return false;
            }
        }

        public bool ClearAllActiveTickets()
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                // Marcăm toate tichetele "On hold" și "Ready" ca "Completed"
                string sql = "UPDATE Tickets SET status = 'Completed' WHERE status IN ('On hold', 'Ready')";
                using var cmd = new MySqlCommand(sql, conn);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error clearing tickets: {ex.Message}");
                return false;
            }
        }
    }
}
