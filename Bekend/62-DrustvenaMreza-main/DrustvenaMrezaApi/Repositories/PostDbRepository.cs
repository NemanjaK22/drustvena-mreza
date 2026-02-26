using DrustvenaMrezaApi.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace DrustvenaMrezaApi.Repositories
{
    public class PostDbRepository
    {
        private readonly string connectionString;

        public PostDbRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public List<Post> GetAll()
        {
            List<Post> posts = new List<Post>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                    SELECT p.Id, p.Content, P.Date, p.UserId,
                    u.Username, u.Name AS Firstname, u.Surname AS Lastname, u.Birthday AS DateOfBirth
                    FROM Posts p
                    INNER JOIN Users u ON p.UserId = u.Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Post post = new Post()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Content = reader["Content"].ToString(),
                        Date = DateTime.ParseExact(reader["Date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Author = new User()
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            FirstName = reader["Firstname"].ToString(),
                            LastName = reader["Lastname"].ToString(),
                            DateOfBirth = DateTime.ParseExact(reader["DateOfBirth"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture)
                        }
                    };
                    posts.Add(post);
                }
                return posts;
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
