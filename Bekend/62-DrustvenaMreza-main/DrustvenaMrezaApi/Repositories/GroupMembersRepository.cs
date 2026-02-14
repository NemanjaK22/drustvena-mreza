
using DrustvenaMrezaApi.Models;

namespace DrustvenaMrezaApi.Repositories
{
    public class GroupMembersRepository
    {

        private const string membershipsFilePath = "data/memberships.csv";
        public static Dictionary<int, List<int>> Data;

        public GroupMembersRepository()
        {
            if (Data == null)
            {
                LoadMemberships();
            }
        }

        private void LoadMemberships()
        {
            Data = new Dictionary<int, List<int>>();
            if (!File.Exists(membershipsFilePath))
            {
                Console.WriteLine("Putanja za ucitavanje clanova grupa ne postoji");
                return;
            }
            try
            {
                string[] lines = File.ReadAllLines(membershipsFilePath);

                foreach (string line in lines)
                {
                    try
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 2)
                        {
                            int userId = int.Parse(parts[0]);
                            int groupId = int.Parse(parts[1]);
                            if (!Data.ContainsKey(groupId))
                            {
                                Data[groupId] = new List<int>();
                            }
                            Data[groupId].Add(userId);
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
                Console.WriteLine("Došlo je do greške prilikom učitavanja članova grupa.");
                Console.WriteLine(e.Message);
            }
        }

        public void SaveMemberships()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(membershipsFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                List<string> lines = new List<string>();
                if (GroupRepository.Data != null)
                {
                    foreach (Group group in GroupRepository.Data.Values)
                    {
                        foreach (User member in group.Members)
                        {
                            lines.Add($"{member.Id},{group.Id}");
                        }
                    }
                }

                File.WriteAllLines(membershipsFilePath, lines);
            }
            catch (Exception e)
            {
                Console.WriteLine("Došlo je do greške prilikom spremanja članova grupa.");
                Console.WriteLine(e.Message);
            }
        }
    }
}
