using DrustvenaMrezaApi.Models;
using Microsoft.Data.Sqlite;

namespace DrustvenaMrezaApi.Repositories
{
    public class UserDbRepository
    {
        private readonly string connectionString = "Data Source=database/database.db";
        
        public List<User> GetAll()
        {
            List<User> users = new List<User>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "SELECT Id, Username, Name, Surname, Birthday FROM Users";
                using SqliteCommand command = new SqliteCommand(query, connection);
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"].ToString(),
                        FirstName = reader["Name"].ToString(),
                        LastName = reader["Surname"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["Birthday"])
                    };
                    users.Add(user);
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
            }
            return users;
        }

        public User GetById(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "SELECT Id, Username, Name, Surname, Birthday FROM Users WHERE Id=@Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int userId = Convert.ToInt32(reader["Id"]);
                    string username = reader["Username"].ToString();
                    string firstName = reader["Name"].ToString();
                    string lastName = reader["Surname"].ToString();
                    DateTime birthday = Convert.ToDateTime(reader["Birthday"]);

                    return new User(userId, username, firstName, lastName, birthday);
                }
                return null;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                return null;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                return null;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                return null;
            }
        }
    }
}
