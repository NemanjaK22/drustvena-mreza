using DrustvenaMrezaApi.Models;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace DrustvenaMrezaApi.Repositories
{
    public class GroupRepository
    {
        private const string filePath = "data/groups.csv";
        public static Dictionary<int, Group> Data;

        public GroupRepository()
        {
            if(Data == null)
            {
                Load();
            }
        }

        private void Load()
        {
            Data = new Dictionary<int, Group>();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] attributes = line.Split(",");
                int id = int.Parse(attributes[0]);
                string name = attributes[1];
                DateTime createdDate = DateTime.ParseExact(attributes[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                Group group = new Group(id, name, createdDate);
                Data[id] = group;

                if (GroupMembersRepository.Data.ContainsKey(id))
                {
                    List<int> userIds = GroupMembersRepository.Data[id];

                    foreach (int userId in userIds)
                    {

                        User user = UserRepository.Data[userId];
                        group.Members.Add(user);
                    }
                }
            }
        }

        public void Save()
        {
            List<string> lines = new List<string>();
            foreach(Group g in Data.Values)
            {
                string formatted = g.CreatedDate.ToString("yyyy-MM-dd");
                lines.Add($"{g.Id},{g.Name},{formatted}");
            }
            File.WriteAllLines(filePath, lines);
        }

    }
}
