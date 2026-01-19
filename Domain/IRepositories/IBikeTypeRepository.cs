using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IBikeTypeRepository
    {
        Task<List<BikeType>> GetAllAsync();
        Task<BikeType?> GetByIdAsync(int typeId);
        Task<BikeType> AddAsync(BikeType bikeType);
        Task UpdateAsync(BikeType bikeType);
        Task DeleteAsync(int typeId);
        Task<bool> ExistsAsync(int typeId);
        Task<bool> IsTypeNameExistAsync(string typeName, int? excludeId = null);
        Task<int> CountBicyclesByTypeAsync(int typeId);
    }
}
