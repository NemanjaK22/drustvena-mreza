using DrustvenaMrezaApi.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace DrustvenaMrezaApi.Repositories
{
    public class UserDbRepository
    {
        private readonly string connectionString;

        public UserDbRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public List<User> GetAll(int page, int pageSize)
        {
            List<User> users = new List<User>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "SELECT Id, Username, Name, Surname, Birthday FROM Users LIMIT @PageSize OFFSET @Offset";
                using SqliteCommand command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"].ToString(),
                        FirstName = reader["Name"].ToString(),
                        LastName = reader["Surname"].ToString(),
                        DateOfBirth = DateTime.ParseExact(reader["Birthday"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    };
                    users.Add(user);
                }
                return users;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public int CountAll()
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users";
                using SqliteCommand command = new SqliteCommand(query, connection);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
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
                if (reader.Read())
                {
                    int userId = Convert.ToInt32(reader["Id"]);
                    string username = reader["Username"].ToString();
                    string firstName = reader["Name"].ToString();
                    string lastName = reader["Surname"].ToString();
                    DateTime birthday = DateTime.ParseExact(reader["Birthday"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    return new User(userId, username, firstName, lastName, birthday);
                }
                return null;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }

        }

        public User Create(User user)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"INSERT INTO Users (Username, Name, Surname, Birthday)
                                 VALUES (@Username, @Name, @Surname, @Birthday);
                                 SELECT LAST_INSERT_ROWID();";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Name", user.FirstName);
                command.Parameters.AddWithValue("@Surname", user.LastName);
                command.Parameters.AddWithValue("@Birthday", user.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                user.Id = Convert.ToInt32(command.ExecuteScalar());
                return user;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }
        public User Update(User updatedUser)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"UPDATE Users
                                 SET Username = @Username, Name = @Name, Surname = @Surname, Birthday = @Birthday
                                  WHERE Id = @Id"; 
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", updatedUser.Id);
                command.Parameters.AddWithValue("@Username", updatedUser.Username);
                command.Parameters.AddWithValue("@Name", updatedUser.FirstName);
                command.Parameters.AddWithValue("@Surname", updatedUser.LastName);
                command.Parameters.AddWithValue("@Birthday", updatedUser.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return updatedUser;
                }
                return null;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "DELETE FROM Users WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }
    }
}
