using DrustvenaMrezaApi.Models;
using DrustvenaMrezaApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace DrustvenaMrezaApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserRepository userRepository = new UserRepository();

        private List<User> GetAllFromDatabase()
        {
            List<User> users = new List<User>();
            string connectionString = "Data Source=database/database.db";

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


        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            List<User> users = GetAllFromDatabase();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            if (UserRepository.Data.ContainsKey(id))
            {
                return Ok(UserRepository.Data[id]);
            }
            else
            {
                return NotFound($"Korisnik sa ID {id} nije pronađen.");
            }
        }

        [HttpPost]
        public ActionResult<User> Create([FromBody] User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Username) || 
                string.IsNullOrWhiteSpace(newUser.FirstName) || 
                string.IsNullOrWhiteSpace(newUser.LastName) || 
                newUser.DateOfBirth == DateTime.MinValue)
            {
                return BadRequest();
            }
            newUser.Id = GetNewId(UserRepository.Data.Keys.ToList());
            UserRepository.Data[newUser.Id] = newUser;
            userRepository.SaveUsers();

            return Ok(newUser);
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update (int id, [FromBody] User updatedUser)
        {
            
            if (string.IsNullOrWhiteSpace(updatedUser.Username) || 
                string.IsNullOrWhiteSpace(updatedUser.FirstName) || 
                string.IsNullOrWhiteSpace(updatedUser.LastName) || 
                updatedUser.DateOfBirth == DateTime.MinValue)
            {
                return BadRequest();
            }
            if (!UserRepository.Data.ContainsKey(id))
            {
                return NotFound($"Korisnik sa ID {id} nije pronađen.");
            }
            User user = UserRepository.Data[id];
            user.Username = updatedUser.Username;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.DateOfBirth = updatedUser.DateOfBirth;
            userRepository.SaveUsers();

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (!UserRepository.Data.ContainsKey(id))
            {
                return NotFound($"Korisnik sa ID {id} nije pronađen.");
            }
            UserRepository.Data.Remove(id);
            userRepository.SaveUsers();

            return NoContent();
        }


        private int GetNewId(List<int> list)
        {
            int maxId = 0;
            foreach (int id in list)
            {
                if (id > maxId)
                {
                    maxId = id;
                }
            }
            return maxId + 1;
        }
    }
}
