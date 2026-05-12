using MySql.Data.MySqlClient;

namespace QManager.DB
{
    public class DbConnection
    {
        private readonly string _connectionString = "Server=localhost;Database=QManager;Uid=root;Pwd=254783;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}