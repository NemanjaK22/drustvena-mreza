using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using DrustvenaMrezaApi.Models;
using DrustvenaMrezaApi.Repositories;

namespace DrustvenaMrezaApi.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private UserRepository userRepository = new UserRepository();
        private GroupMembersRepository membershipRepository = new GroupMembersRepository();
        private GroupRepository groupRepository = new GroupRepository();
        //GET api/groups
        [HttpGet]
        public ActionResult<List<Group>> GetAll()
        {
            List<Group> groups = GroupRepository.Data.Values.ToList();

            return Ok(groups);
        }

        [HttpPost]
        public ActionResult<List<Group>>Create([FromBody] Group newGroup)
        {
            if(string.IsNullOrWhiteSpace(newGroup.Name))
            {
                return BadRequest();
            }
            newGroup.Id = SracunajNoviId(GroupRepository.Data.Keys.ToList());
            GroupRepository.Data[newGroup.Id] = newGroup;
            groupRepository.Save();

            return Ok(newGroup);
;       }
        private int SracunajNoviId(List<int> identifikatori)
        {
            int maxId = 0;
            foreach(int id in identifikatori)
            {
                if(id > maxId)
                {
                    maxId = id;
                }
            }
            return maxId + 1;
        }

        //DELETE api/groups/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if(!GroupRepository.Data.ContainsKey(id))
            {
                return NotFound();
            }
            GroupRepository.Data.Remove(id);
            groupRepository.Save();

            return NoContent();
        }

    }
}
