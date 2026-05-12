using MySql.Data.MySqlClient;
using QManager.Models;

namespace QManager.DB.Repositories
{
    public class AdminRepository
    {
        private readonly DbConnection _db = new DbConnection();

        public Admin? Login(string username, string password)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                // Nota: in productie foloseste hashing pentru parole.
                string sql = "SELECT * FROM Admins WHERE username = @user AND password_hash = @pass";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Admin {
                        AdminId = reader.GetInt32("admin_id"),
                        Username = reader.GetString("username"),
                        Theme = reader.GetString("theme") == "Dark" ? UserTheme.Dark : UserTheme.Light
                    };
                }
            }
            catch (MySqlException)
            {
                return null;
            }

            return null;
        }
    }
}
