using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using firstprojectcontextlib;
using System.Linq;
using System.Security.Claims;
using System.Text;
using firstProjectApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Protector.Protector;

namespace firstProjectApi.Repos
{
    public class UsersRepo : IUsersRepo
    {
        private readonly FirstProject _db;
        public UsersRepo(FirstProject db)
        {
            this._db = db;
        }
        
        public async Task<User> CreateAsync(User user)
        {
            user.Username = user.Username.ToUpper();
            user = UserWithHashedPasswordAndSalt(user);
            await _db.Users.AddAsync(user);
            int affected = await _db.SaveChangesAsync();
            return affected == 1 ? user : null;
        }

        public Task<IEnumerable<User>> RetrieveAllAsync()
        {
            return Task.Run<IEnumerable<User>>(() => _db.Users);
        }

        public Task<User> RetrieveAsync(int id)
        {
            return Task.Run(() =>
            {
                return _db.Users.Where(user => user.UserId == id).FirstAsync();
            });
        }

        public async Task<User> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            int affected =await  _db.SaveChangesAsync();
            return affected == 1 ? user : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            User user = await _db.Users.FindAsync(id);
            _db.Users.Remove(user);
            int affected = await _db.SaveChangesAsync();
            return affected == 1;
        }

        public async Task<dynamic> Authenticate(AuthenticateModel model)
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username.Equals(model.Username.ToUpper()));
            if (user == null) return null;
            if (CheckPassword(model.Password, user.Password, user.Salt))
            {
                return GenerateToken(user);
            }
            else
            {
                return null;
            }
        } 

        public dynamic GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name , user.Username),
                new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),
                //nbf = not before
                new Claim(JwtRegisteredClaimNames.Nbf,new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.Role , user.Role?? "user")
            };

            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.Unicode.GetBytes("youssefsecret")), SecurityAlgorithms.HmacSha256)
                    ),
                new JwtPayload(claims)
                );
            var output = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                username = user.Username
            };

            return output;

        }
    }
}