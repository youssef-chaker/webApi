using System;
using Microsoft.AspNetCore.Mvc;
using firstProjectApi.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using firstprojectcontextlib;

namespace firstProjectApi.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private IUsersRepo repo;

        public CustomersController(IUsersRepo repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await repo.RetrieveAllAsync();
        }

        [HttpGet("{id}", Name = nameof(GetUser))]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUser(int id)
        {
            User user = await repo.RetrieveAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            User addedUser = await repo.CreateAsync(user);
            return CreatedAtRoute(routeName: nameof(GetUser), routeValues: new {id = addedUser.UserId},
                value: addedUser);
        }

        [HttpPut("{id}")]
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

            var existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await repo.UpdateAsync(user);
            return NoContent();
            
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            bool deleted = await repo.DeleteAsync(id);

            if (deleted)
            {
                return new NoContentResult();
            }
            else
            {
                return BadRequest("some error could not delete user");
            }
            
        }

        
    }
}