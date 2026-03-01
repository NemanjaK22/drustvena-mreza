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
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            try
            {
                Group group = groupDbRepository.getById(id);

                if (group == null)
                {
                    return NotFound($"Grupa sa ID {id} nije pronađena.");
                }

                return Ok(group);
            }
            catch (Exception)
            {
                return Problem($"Došlo je do greške prilikom dobavljanja grupe sa ID {id}.");
            }
        }


        [HttpPost]
        public ActionResult<List<Group>> Create([FromBody] Group newGroup)
        {
            if (string.IsNullOrWhiteSpace(newGroup.Name))
            {
                return BadRequest("Nisu uneti svi obavezni podaci");
            }
            try
            {
                Group createdGroup = groupDbRepository.Create(newGroup);
                return Ok(createdGroup);
            }
            catch (Exception)
            {
                return Problem("Doslo je do greske prilikom kreiranja grupe u bazi podataka");
            }
        }
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] Group updatedGroup)
        {
            if(string.IsNullOrWhiteSpace(updatedGroup.Name))
            {
                return BadRequest();
            }
            try
            {
                updatedGroup.Id = id;
                Group group = groupDbRepository.Update(updatedGroup);
                if (group == null)
                {
                    return NotFound($"Grupa sa Id {id} nije pronadjena");
                }
                return Ok(group);
            }
            catch (Exception)
            {
                return Problem($"Doslo je do greske prilikom azuriranja grupe sa ID {id}.");
            }
        }

        //DELETE api/groups/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = groupDbRepository.Delete(id);
                if(!isDeleted)
                {
                    return NotFound($"Korisnik sa ID {id} nije pronadjen");
                }
                return NoContent();
            }
            catch(Exception)
            {
                return Problem($"Doslo je do greske prilikom brisanja korisnika sa ID {id}.");
            }
        }

    }
}
