using System.Collections.Generic;
using System.Threading.Tasks;
using firstprojectcontextlib;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace firstProjectApi.Repos
{
    public class PostsRepo : IPostsRepo
    {
        private readonly FirstProject _db;

        public PostsRepo(FirstProject db)
        {
            _db = db;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            await _db.Posts.AddAsync(post);
            int affected =await  _db.SaveChangesAsync();
            return affected == 1 ? post : null;
        }

        public Task<IEnumerable<Post>> RetrieveAllAsync()
        {
            return Task.Run<IEnumerable<Post>>(() => _db.Posts);
        }

        public async Task<Post> RetrieveAsync(int id)
        {
            return await _db.Posts.Where(post => post.PostId == id).FirstOrDefaultAsync();
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            _db.Posts.Update(post);
            int affected =await  _db.SaveChangesAsync();
            return affected == 1 ? post : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Post post = await _db.Posts.FindAsync(id);
            _db.Posts.Remove(post);
            int affected = await _db.SaveChangesAsync();
            return affected == 1;
            
        }
    }
}