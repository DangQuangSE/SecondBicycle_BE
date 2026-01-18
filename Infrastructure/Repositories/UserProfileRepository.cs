using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;

        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserProfile>> GetAllAsync()
        {
            return await _context.UserProfiles.AsNoTracking().ToListAsync();
        }
        public async Task<UserProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.UserProfiles
                                 .FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task AddAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
        }
    }
}
