using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        readonly DataContext _db;

        public AuthRepository(DataContext context)
        {
            _db = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(t => t.Username == username);
            if(user == null)
            {
                return null;
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); //convert string password to byte array
                
                if(computedHash.Length != passwordHash.Length)
                    return false;   

                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    } 
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);   
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _db.Users.AddAsync(user);      
            await _db.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); //convert string password to byte array
            }            
        }

        public async Task<bool> UserExists(string username)
        {
            // if(await _db.Users.AnyAsync(t => t.Username == username))
            // {
            //     return true;
            // }
            // return false;

            return await _db
                .Users
                .AnyAsync(t => t.Username == username);
            
        }
    }
}