using System.Collections.Generic;
using System.Threading.Tasks;
using firstprojectcontextlib;

namespace firstProjectApi.Repos
{
    public interface IUsersRepo
    {
        Task<User> CreateAsync(User user);
        Task<IEnumerable<User>> RetrieveAllAsync();
        Task<User> RetrieveAsync(int id);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
    }
}