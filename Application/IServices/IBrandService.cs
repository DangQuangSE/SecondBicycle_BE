using Application.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IBrandService
    {
        Task<List<BrandViewDto>> GetAllAsync();
        Task<BrandViewDto?> GetByIdAsync(int id);
        Task<BrandViewDto> CreateAsync(BrandCreateDto dto);
        Task<BrandViewDto?> UpdateAsync(int id, BrandUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
