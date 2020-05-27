using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        readonly DataContext _db;

        public DatingRepository(DataContext context)
        {
            _db = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _db.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _db.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _db.Users
                .Include(t => t.Photos)
                .FirstOrDefaultAsync(t => t.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _db.Users
                .Include(t => t.Photos)
                .ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}