using MySql.Data.MySqlClient;
using System;

namespace QManager.DB

{
    public class DbConnection
    {
        // Datele tale de conectare din MySQL Workbench
        private readonly string _connectionString = "Server=localhost;Database=QManager;Uid=root;Pwd=254783;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Metodă simplă să verifici dacă merge conexiunea
        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true; // Totul e ok
                }
            }
            catch (Exception ex)
            {
                // Aici va apărea eroarea dacă parola sau numele bazei de date e greșit
                Console.WriteLine("Eroare: " + ex.Message);
                return false;
            }
        }
    }
}
