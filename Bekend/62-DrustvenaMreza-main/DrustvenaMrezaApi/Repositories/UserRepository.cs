using DrustvenaMrezaApi.Models;
using System.Globalization;

namespace DrustvenaMrezaApi.Repositories
{
    public class UserRepository
    {

        private const string filePath = "data/users.csv";
        public static Dictionary<int, User> Data;

        public UserRepository()
        {
            if (Data == null)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            Data = new Dictionary<int, User>();
            
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Putanja za ucitavanje korisnika ne postoji");
                    return;
                }
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    try
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 5)
                        {
                            int id = int.Parse(parts[0]);
                            string username = parts[1];
                            string firstName = parts[2];
                            string lastName = parts[3];
                            DateTime dateOfBirth = DateTime.ParseExact(parts[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            User user = new(id, username, firstName, lastName, dateOfBirth);
                            Data[id] = user;
                        }
                    }
                    catch (Exception)
                    {

                        Console.WriteLine($"Greska pri parsiranju linije: {line}. Preskacem red.");
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Došlo je do greške prilikom učitavanja korisnika.");
                Console.WriteLine(e.Message);
            }
        }

        public void SaveUsers()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                List<string> lines = new List<string>();
                foreach (User user in Data.Values)
                {
                    string dateOfBirth = user.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    string line = $"{user.Id},{user.Username},{user.FirstName},{user.LastName},{dateOfBirth}";
                    lines.Add(line);
                }
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception e)
            {
                Console.WriteLine("Došlo je do greške prilikom čuvanja korisnika.");
                Console.WriteLine(e.Message);
            }
        }
    }
}
