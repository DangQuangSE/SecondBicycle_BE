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
    public class BikeTypeRepository : IBikeTypeRepository
    {
        private readonly AppDbContext _context;

        public BikeTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BikeType>> GetAllAsync()
        {
            return await _context.BikeTypes
                .Include(bt => bt.Bicycles)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BikeType?> GetByIdAsync(int typeId)
        {
            return await _context.BikeTypes
                .Include(bt => bt.Bicycles)
                .FirstOrDefaultAsync(bt => bt.TypeId == typeId);
        }

        public async Task<BikeType> AddAsync(BikeType bikeType)
        {
            await _context.BikeTypes.AddAsync(bikeType);
            await _context.SaveChangesAsync();
            return bikeType;
        }

        public async Task UpdateAsync(BikeType bikeType)
        {
            _context.BikeTypes.Update(bikeType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int typeId)
        {
            var bikeType = await _context.BikeTypes.FindAsync(typeId);
            if (bikeType != null)
            {
                _context.BikeTypes.Remove(bikeType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int typeId)
        {
            return await _context.BikeTypes.AnyAsync(bt => bt.TypeId == typeId);
        }

        public async Task<bool> IsTypeNameExistAsync(string typeName, int? excludeId = null)
        {
            var query = _context.BikeTypes.Where(bt => bt.TypeName.ToLower() == typeName.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(bt => bt.TypeId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> CountBicyclesByTypeAsync(int typeId)
        {
            return await _context.Bicycles.CountAsync(b => b.TypeId == typeId);
        }
    }
}
