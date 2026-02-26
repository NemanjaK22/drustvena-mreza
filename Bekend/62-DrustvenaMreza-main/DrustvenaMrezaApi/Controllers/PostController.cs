using DrustvenaMrezaApi.Models;
using DrustvenaMrezaApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustvenaMrezaApi.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbRepository postDbRepository;
        private readonly UserDbRepository userDbRepository;

        public PostController(IConfiguration configuration)
        {
            postDbRepository = new PostDbRepository(configuration);
            userDbRepository = new UserDbRepository(configuration);
        }

        [HttpGet]
        public ActionResult<List<PostController>> GetAll()
        {
            try
            {
                List<Post> posts = postDbRepository.GetAll();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return Problem("Doslo je do greske prilikom dobavljanja objava: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<Post> Create([FromBody] Post newPost)
        {
            if (string.IsNullOrWhiteSpace(newPost.Content))
            {
                return BadRequest("Sadržaj objave ne sme biti prazan.");
            }
            if (newPost.Date == DateTime.MinValue)
            {
                newPost.Date = DateTime.Now;
            }
            try
            {
                Post createdPost = postDbRepository.Create(newPost);
                User author = userDbRepository.GetById(createdPost.UserId);
                createdPost.Author = author;

                return Ok(createdPost);
            }
            catch (Exception ex)
            {
                return Problem("Doslo je do greske prilikom kreiranja objave: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = postDbRepository.Delete(id);
                if (isDeleted)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Objava sa zadatim ID {id} nije pronađena.");
                }
            }
            catch (Exception ex)
            {
                return Problem("Doslo je do greske prilikom brisanja objave: " + ex.Message);
            }
        }
    }
}
