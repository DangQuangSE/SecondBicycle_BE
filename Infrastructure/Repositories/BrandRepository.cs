using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context) => _context = context;

        public async Task<List<Brand>> GetAllAsync()
        {
            try
            {
                return await _context.Brands.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving brands from database", ex);
            }
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving brand with ID {id}", ex);
            }
        }

        public async Task AddAsync(Brand brand)
        {
            try
            {
                await _context.Brands.AddAsync(brand);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding brand to database", ex);
            }
        }

        public Task UpdateAsync(Brand brand)
        {
            try
            {
                _context.Brands.Update(brand);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating brand with ID {brand.BrandId}", ex);
            }
        }

        public Task DeleteAsync(Brand brand)
        {
            try
            {
                _context.Brands.Remove(brand);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting brand with ID {brand.BrandId}", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ValidationException("The brand was modified or deleted by another user. Please refresh and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                // Check for unique constraint violations
                if (ex.InnerException?.Message.Contains("UNIQUE") == true ||
                    ex.InnerException?.Message.Contains("duplicate") == true)
                {
                    throw new ValidationException("A brand with this name already exists.", ex);
                }

                // Check for foreign key violations
                if (ex.InnerException?.Message.Contains("FOREIGN KEY") == true ||
                    ex.InnerException?.Message.Contains("REFERENCE") == true)
                {
                    throw new ValidationException("Cannot delete this brand because it is being used by other records.", ex);
                }

                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while saving to database.", ex);
            }
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Brands.AnyAsync(b => b.BrandId == id);
        }

        public async Task<bool> ExistsByNameAsync(string brandName)
        {
            return await _context.Brands.AnyAsync(b => b.BrandName == brandName);
        }
        public async Task<Brand?> GetByNameAsync(string brandName)
        {
            try
            {
                return await _context.Brands
                    .FirstOrDefaultAsync(b => b.BrandName == brandName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving brand with name '{brandName}'", ex);
            }
        }

    }
}