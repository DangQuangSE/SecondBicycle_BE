using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(int id);
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByNameAsync(string brandName);
        Task<Brand?> GetByNameAsync(string brandName);

    }
}
