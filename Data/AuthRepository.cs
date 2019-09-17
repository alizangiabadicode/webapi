using System;
using System.Threading.Tasks;
using datingapp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            User user = await context.Users.Include(e=> e.Photos).FirstOrDefaultAsync(e=> e.UserName == username.ToLower());

            if(user == null){
                return null;
            }

            if(!ValidatePass(password,user.PasswordSalt,user.PasswordHash)){
                return null;
            }
            
            return user;
        }

        private bool ValidatePass(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i =0 ; i< pass.Length; i++)
                {
                    if(pass[i] != passwordHash[i]){
                        return false;
                    }
                }
            }
            
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordhash, passwordsalt;
            CreatePasswordHash(password, out passwordhash, out passwordsalt);

            user.PasswordHash = passwordhash;
            user.PasswordSalt = passwordsalt;

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await context.Users.AnyAsync(e=> e.UserName == username)){
                return true;
            }
            
            return false;
        }
    }
}