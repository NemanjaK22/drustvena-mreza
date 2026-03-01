using DrustvenaMrezaApi.Models;
using Microsoft.AspNetCore.Mvc;
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
        public Group Create(Group group)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString); 
                connection.Open();

                string query = @"INSERT INTO Groups(Name,CreationDate)
                                 VALUES(@Name,@CreationDate);
                                 SELECT LAST_INSERT_ROWID()";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", group.Name);
                command.Parameters.AddWithValue("@CreationDate", group.CreatedDate);
                group.Id = Convert.ToInt32(command.ExecuteScalar());
                return group;
            }
            catch(SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili pri izvrsavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta {ex.Message}");
                throw;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Neocekivana greska{ex.Message}");
                throw;
            }
        }
        public Group Update(Group updatedGroup)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection();
                connection.Open();
                string query = @"UPDATE Groups SET Name = @Name, CreationDate = @CreationDate
                                WHERE Id = Id";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Name",updatedGroup.Name);
                command.Parameters.AddWithValue("@CreationDate", updatedGroup.CreatedDate);

                int rowsAffected = command.ExecuteNonQuery();
                {
                    if (rowsAffected > 0)
                    {
                        return updatedGroup;
                    }
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
                string query = "DELETE FROM Groups WHERE Id = @Id";
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