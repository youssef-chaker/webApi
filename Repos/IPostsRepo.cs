using System.Collections.Generic;
using System.Threading.Tasks;
using firstProjectApi.Models;
using firstprojectcontextlib;

namespace firstProjectApi.Repos
{
    public interface IPostsRepo
    {
        Task<Post> CreateAsync(Post post);
        Task<IEnumerable<Post>> RetrieveAllAsync();
        Task<Post> RetrieveAsync(int id);
        Task<Post> UpdateAsync(Post post);
        Task<bool> DeleteAsync(int id);
    }
}