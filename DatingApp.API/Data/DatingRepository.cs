using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DatingApp.API.Helpers;
using System;

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

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _db.Photos
                .FirstOrDefaultAsync(t => t.UserId == userId && t.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _db.Photos
                .FirstOrDefaultAsync(t => t.Id == id);
                
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _db.Users
                .Include(t => t.Photos)
                .FirstOrDefaultAsync(t => t.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _db.Users
                .Include(t => t.Photos)
                .OrderByDescending(t => t.LastActive)   //sort by most recently active first
                .AsQueryable();

            users = users.Where(t => t.Id != userParams.UserId);
            users = users.Where(t => t.Gender == userParams.Gender);
            
            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(t => t.DateOfBirth >= minDoB && t.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy){
                    case "created" :
                        users = users.OrderByDescending(t => t.Created);
                        break;
                    default:
                        users = users.OrderByDescending(t => t.LastActive);
                        break;
                }
            }

            return await PagedList<User>
                .CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}