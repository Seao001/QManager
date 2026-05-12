using MySql.Data.MySqlClient;
using QManager.Models;
using System.Collections.Generic;

namespace QManager.DB.Repositories
{
    public class ServiceRepository
    {
        private readonly DbConnection _db = new DbConnection();

        public List<QueueService> GetAllServices()
        {
            var services = new List<QueueService>();

            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                string sql = "SELECT * FROM Services";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    services.Add(new QueueService {
                        ServiceId = reader.GetInt32("service_id"),
                        ServiceName = reader.GetString("service_name")
                    });
                }
            }
            catch (MySqlException)
            {
                return services;
            }

            return services;
        }
    }
}
