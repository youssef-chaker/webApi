using Microsoft.AspNetCore.Mvc;
using firstProjectApi.Repos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using firstProjectApi.Models;
using firstprojectcontextlib;
using Microsoft.AspNetCore.Authorization;

namespace firstProjectApi.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _repo;

        public UsersController(IUsersRepo repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _repo.RetrieveAllAsync();
        }

        [HttpGet("{id}", Name = nameof(GetUser))]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUser(int id)
        {
            User user = await _repo.RetrieveAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Salt = null;
            return Ok(user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] RegisterModel model)
        {
            if (model == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            User user = new User
            {
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                Role = "user"
            };
            User addedUser = await _repo.CreateAsync(user);
            return CreatedAtRoute(routeName: nameof(GetUser), routeValues: new {id = addedUser.UserId},
                value: _repo.GenerateToken(user));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (user == null || id != user.UserId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _repo.UpdateAsync(user);
            return NoContent();
            
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = _repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            bool deleted = await _repo.DeleteAsync(id);

            if (deleted)
            {
                return new NoContentResult();
            }
            else
            {
                return BadRequest("some error could not delete user");
            }
            
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            var tokenObject = await _repo.Authenticate(model);
            if (tokenObject == null) return BadRequest(new {message = "username or password is incorrect / user does not exist"});
            return new ObjectResult(await _repo.Authenticate(model));
        }

        
    }
}
