using DrustvenaMrezaApi.Models;
using DrustvenaMrezaApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustvenaMrezaApi.Controllers
{
    [Route("api/groups/{groupId}/users")]
    [ApiController]
    public class GroupMembersController : ControllerBase
    {

        private UserRepository userRepository = new UserRepository();
        private GroupMembersRepository membershipRepository = new GroupMembersRepository();
        private GroupRepository groupRepository = new GroupRepository();

        [HttpGet]
        public ActionResult<List<User>> GetUsersByGroup(int groupId)
        {
            if (!GroupRepository.Data.ContainsKey(groupId))
            {
                 return NotFound("Group not found.");
            }
            Group group = GroupRepository.Data[groupId];
            return Ok(group.Members);

        }

        //Put api/groups/{groupId}/groups/{groupId}
        [HttpPut("{userId}")]
        public ActionResult<User> Add(int userId, int groupId)
        {
            if (!GroupRepository.Data.ContainsKey(groupId))
            {
                return NotFound("Group not found!");
            }
            if(!UserRepository.Data.ContainsKey(userId))
            {
                return NotFound("User not found!");
            }
            Group group = GroupRepository.Data[groupId];
            foreach (User member in group.Members)
            {
                if (member.Id == userId)
                {
                    return Conflict();
                }
            }
            User user = UserRepository.Data[userId];
            group.Members.Add(user);
            membershipRepository.SaveMemberships();

            return Ok(group);

        }
        [HttpDelete("{userId}")]
        public ActionResult<Group> Remove(int groupId, int userId)
        {
            if (!GroupRepository.Data.ContainsKey(groupId))
            {
                return NotFound("Group not found");
            }

            if (!UserRepository.Data.ContainsKey(userId))
            {
                return NotFound("User not found");
            }

            Group group = GroupRepository.Data[groupId];
            User user = UserRepository.Data[userId];
            group.Members.Remove(user);
            // Sačuvamo podatke o članstvima
            membershipRepository.SaveMemberships();

            return Ok(group);
        }

    }
}
