using DrustvenaMrezaApi.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace DrustvenaMrezaApi.Repositories
{
    public class GroupDbRepository
    {
        private readonly string connectionString;

        public GroupDbRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public List<Group> GetAll(int page, int pageSize)
        {
            List<Group> groups = new List<Group>();
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "SELECT Id, Name, CreationDate FROM Groups LIMIT @PageSize OFFSET @Offset";
                using SqliteCommand command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Group group = new Group
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        CreatedDate = reader["CreationDate"] == DBNull.Value
                        ? DateTime.MinValue
                        : DateTime.Parse(reader["CreationDate"].ToString(), CultureInfo.InvariantCulture)
                    };
                    groups.Add(group);
                }
                return groups;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch(FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze:{ex.Message}");
                throw;
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta:{ex.Message}");
                throw;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }
        }

        public Group getById(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "SELECT Id, Name, CreationDate FROM Groups WHERE Id=@Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                using SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int Id = Convert.ToInt32(reader["Id"]);
                    string Name = reader["Name"].ToString();
                    DateTime CreatedDate = reader["CreationDate"] == DBNull.Value
                        ? DateTime.MinValue
                        : DateTime.Parse(reader["CreationDate"].ToString(), CultureInfo.InvariantCulture);
                    return new Group(Id, Name, CreatedDate);
                }
                return null;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neispravnih SQL upita:{ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze:{ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska:{ex.Message}");
                throw;
            }
        }
    }
}