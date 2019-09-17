using System.Collections.Generic;
using System.IO;
using datingapp.api.Models;
using Newtonsoft.Json;

namespace datingapp.api.Data
{
    public class Seed
    {
        private readonly DataContext context;
        public Seed(DataContext context)
        {
            this.context = context;
        }

        public void StartSeed()
        {
            var userStrings= File.ReadAllText("Data/UserSeedData.json");
            var users= JsonConvert.DeserializeObject<List<User>>(userStrings);
            foreach(User user in users){
                user.UserName= user.UserName.ToLower();
                string password = "password";
                byte[] passwordhash, passwordsalt;
                CreatePasswordHash(password, out passwordhash, out passwordsalt);
                user.PasswordHash= passwordhash;
                user.PasswordSalt= passwordsalt;
                context.Users.Add(user);
            }

            context.SaveChanges();
        }

        
        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}