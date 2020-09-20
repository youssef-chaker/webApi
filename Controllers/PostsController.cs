using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using firstProjectApi.Repos;
using firstprojectcontextlib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace firstProjectApi.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsRepo _repo;

        public PostsController(IPostsRepo repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Post>))]
        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _repo.RetrieveAllAsync();
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(Post))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPost(int id)
        {
            Post post = await _repo.RetrieveAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        [Authorize(Roles = "user,admin")]
        [ProducesResponseType((201))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] Post post)
        {
            if (post is null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addedPost = await _repo.CreateAsync(post);
            if (addedPost is null)
            {
                return BadRequest();
            }

            Console.WriteLine(addedPost.PostId);

            return CreatedAtRoute(routeName: nameof(GetPost), routeValues: new {id = addedPost.PostId},
                value: post);

        }
        
    }
}