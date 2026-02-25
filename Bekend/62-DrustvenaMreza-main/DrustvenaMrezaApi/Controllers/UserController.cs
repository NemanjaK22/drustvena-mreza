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
        private readonly UserDbRepository userDbRepository;

        public UserController(IConfiguration configuration)
        {
            userDbRepository = new UserDbRepository(configuration);
        }



        [HttpGet]
        public ActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page i pageSize moraju biti veći od 0.");
            }
            try
            {
                List<User> users = userDbRepository.GetAll(page, pageSize);
                int totalCount = userDbRepository.CountAll();

                Object result = new
                {
                    Data = users,
                    TotalCount = totalCount
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return Problem("Došlo je do greške prilikom dobavljanja svih korisnika iz baze.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            try
            {
                User user = userDbRepository.GetById(id);

                if (user == null)
                {
                    return NotFound($"Korisnik sa ID {id} nije pronađen.");
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return Problem($"Došlo je do greške prilikom dobavljanja korisnika sa ID {id}.");
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
                return BadRequest("Nisu uneti svi obavezni podaci.");
            }

            try
            {
                User createdUser = userDbRepository.Create(newUser);
                return Ok(createdUser);
            }
            catch (Exception)
            {
                return Problem("Došlo je do greške prilikom kreiranja korisnika u bazi podataka.");
            }
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
            try
            {
                updatedUser.Id = id;
                User user = userDbRepository.Update(updatedUser);
                if (user == null)
                {
                    return NotFound($"Korisnik sa ID {id} nije pronađen.");
                }
                return Ok(user);
            }
            catch (Exception)
            {
                return Problem($"Došlo je do greške prilikom ažuriranja korisnika sa ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = userDbRepository.Delete(id);
                if (!isDeleted)
                {
                    return NotFound($"Korisnik sa ID {id} nije pronađen.");
                }
                return NoContent();
            }
            catch (Exception)
            {
                return Problem($"Došlo je do greške prilikom brisanja korisnika sa ID {id}.");
            }
        }
    }
}
