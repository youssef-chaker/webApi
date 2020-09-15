using System.Collections.Generic;
using System.Threading.Tasks;
using firstProjectApi.Models;
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
        Task<dynamic> Authenticate(AuthenticateModel model);
        dynamic GenerateToken(User user);
    }
}