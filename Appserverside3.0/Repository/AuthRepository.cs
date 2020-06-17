using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppServerSide.Data;
using AppServerSide.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AppServerSide.Repository
{
    public class AuthRepository:IAuthRepository
    {
        private DataContext _context;

        #region private methods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            byte[] key=new byte[16];
            RandomNumberGenerator.Create().GetBytes(key);
            using (var encrypt = SHA256.Create())
            {
                passwordSalt = key;
                passwordHash = encrypt.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password+Convert.ToBase64String(key)));

            }
            
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var encrypt = SHA256.Create();

            return Encoding.ASCII.GetString(encrypt.ComputeHash(Encoding.ASCII.GetBytes(password+Convert.ToBase64String(passwordSalt)))) == Encoding.ASCII.GetString(passwordHash);
        }

        #endregion
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user, string password){

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password,out passwordHash,out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
     
        }

      
        public async Task<User> Login(string username, string password){

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
                return null;
            else if(VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                return user;
            }
            else{
                return null;
            }
           
        }

      
        public async Task<bool> UserExists(string username){

            return await _context.Users.AnyAsync(x => x.Username == username);
           
        }

    }
}
