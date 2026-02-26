using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using DrustvenaMrezaApi.Models;
using DrustvenaMrezaApi.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace DrustvenaMrezaApi.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private UserRepository userRepository = new UserRepository();
        private GroupMembersRepository membershipRepository = new GroupMembersRepository();
        private GroupRepository groupRepository = new GroupRepository();
        private readonly GroupDbRepository groupDbRepository;

        public GroupController(IConfiguration configuration)
        {
            groupDbRepository = new GroupDbRepository(configuration);
        }
        //GET api/groups
        [HttpGet]
        public ActionResult<List<Group>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page i pageSize moraju biti veći od 0.");
            }
            try
            {
                List<Group> groups = groupDbRepository.GetAll(page, pageSize);

                Object result = new
                {
                    Data = groups
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.ToString(), statusCode: 500);
            }

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
