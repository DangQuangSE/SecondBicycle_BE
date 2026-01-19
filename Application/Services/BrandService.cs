using Application.DTOs.Brand;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repo;

        public BrandService(IBrandRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<BrandViewDto>> GetAllAsync()
        {
            var brands = await _repo.GetAllAsync();
            return brands.Select(b => new BrandViewDto
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                Country = b.Country
            }).ToList();
        }

        public async Task<BrandViewDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Brand ID must be greater than 0");

            var brand = await _repo.GetByIdAsync(id);
            if (brand == null)
                throw new KeyNotFoundException($"Brand with ID {id} not found");

            return new BrandViewDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Country = brand.Country
            };
        }

        public async Task<BrandViewDto> CreateAsync(BrandCreateDto dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.BrandName))
                throw new ValidationException("Brand name is required");

            if (dto.BrandName.Length > 100)
                throw new ValidationException("Brand name must not exceed 100 characters");

            // Check duplicate
            if (await _repo.ExistsByNameAsync(dto.BrandName))
                throw new ValidationException($"Brand with name '{dto.BrandName}' already exists");

            var brand = new Brand
            {
                BrandName = dto.BrandName.Trim(),
                Country = dto.Country?.Trim()
            };

            await _repo.AddAsync(brand);
            await _repo.SaveChangesAsync();

            return new BrandViewDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Country = brand.Country
            };
        }

        public async Task<BrandViewDto> UpdateAsync(int id, BrandUpdateDto dto)
        {
            if (id <= 0)
                throw new ValidationException("Brand ID must be greater than 0");

            // Validation
            if (string.IsNullOrWhiteSpace(dto.BrandName))
                throw new ValidationException("Brand name is required");

            if (dto.BrandName.Length > 100)
                throw new ValidationException("Brand name must not exceed 100 characters");

            var brand = await _repo.GetByIdAsync(id);
            if (brand == null)
                throw new KeyNotFoundException($"Brand with ID {id} not found");

            // Nếu đổi tên và tên mới đã tồn tại → lỗi
            if (brand.BrandName != dto.BrandName.Trim() &&
                await _repo.ExistsByNameAsync(dto.BrandName))
            {
                throw new ValidationException($"Brand with name '{dto.BrandName}' already exists");
            }

            brand.BrandName = dto.BrandName.Trim();
            brand.Country = dto.Country?.Trim();

            await _repo.UpdateAsync(brand);
            await _repo.SaveChangesAsync();

            return new BrandViewDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Country = brand.Country
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Brand ID must be greater than 0");

            var brand = await _repo.GetByIdAsync(id);
            if (brand == null)
                throw new KeyNotFoundException($"Brand with ID {id} not found");

            await _repo.DeleteAsync(brand);
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}
