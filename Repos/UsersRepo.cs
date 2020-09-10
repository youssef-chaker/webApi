using System.Collections.Generic;
using System.Threading.Tasks;
using firstprojectcontextlib;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace firstProjectApi.Repos
{
    public class UsersRepo : IUsersRepo
    {
        private FirstProject db;
        public UsersRepo(FirstProject db)
        {
            this.db = db;
        }
        
        public async Task<User> CreateAsync(User user)
        {
            user.Username = user.Username.ToUpper();
            EntityEntry<User> added = await db.Users.AddAsync(user);
            int affected = await db.SaveChangesAsync();
            return affected == 1 ? user : null;
        }

        public Task<IEnumerable<User>> RetrieveAllAsync()
        {
            return Task.Run<IEnumerable<User>>(() => db.Users);
        }

        public Task<User> RetrieveAsync(int id)
        {
            return Task.Run(() =>
            {
                return db.Users.Where(user => user.UserId == id).FirstAsync();
            });
        }

        public async Task<User> UpdateAsync(User user)
        {
            db.Users.Update(user);
            int affected =await  db.SaveChangesAsync();
            return affected == 1 ? user : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            int affected = await db.SaveChangesAsync();
            return affected == 1;
        }
    }
}