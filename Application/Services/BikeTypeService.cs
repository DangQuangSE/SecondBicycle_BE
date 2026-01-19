using Application.DTOs.BikeType;
using Application.Helpers;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BikeTypeService : IBikeTypeService
    {
        private readonly IBikeTypeRepository _repository;

        public BikeTypeService(IBikeTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<GenericResult<List<BikeTypeDto>>> GetAllBikeTypesAsync()
        {
            var bikeTypes = await _repository.GetAllAsync();

            var bikeTypeDtos = bikeTypes.Select(bt => new BikeTypeDto
            {
                TypeId = bt.TypeId,
                TypeName = bt.TypeName,
                BicycleCount = bt.Bicycles.Count
            }).ToList();

            return GenericResult<List<BikeTypeDto>>.Success(bikeTypeDtos);
        }

        public async Task<GenericResult<BikeTypeDto>> GetBikeTypeByIdAsync(int typeId)
        {
            var bikeType = await _repository.GetByIdAsync(typeId);
            
            if (bikeType == null)
            {
                return GenericResult<BikeTypeDto>.Failure("Bike type not found");
            }

            var bikeTypeDto = new BikeTypeDto
            {
                TypeId = bikeType.TypeId,
                TypeName = bikeType.TypeName,
                BicycleCount = bikeType.Bicycles.Count
            };

            return GenericResult<BikeTypeDto>.Success(bikeTypeDto);
        }

        public async Task<GenericResult<BikeTypeDto>> CreateBikeTypeAsync(CreateBikeTypeDto dto)
        {
            // Ki?m tra tên ð? t?n t?i chýa
            var isExist = await _repository.IsTypeNameExistAsync(dto.TypeName);
            if (isExist)
            {
                return GenericResult<BikeTypeDto>.Failure("Bike type name already exists");
            }

            // Validate tên không ðý?c r?ng
            if (string.IsNullOrWhiteSpace(dto.TypeName))
            {
                return GenericResult<BikeTypeDto>.Failure("Bike type name is required");
            }

            // T?o entity m?i
            var newBikeType = new BikeType
            {
                TypeName = dto.TypeName.Trim()
            };

            var createdBikeType = await _repository.AddAsync(newBikeType);

            var resultDto = new BikeTypeDto
            {
                TypeId = createdBikeType.TypeId,
                TypeName = createdBikeType.TypeName,
                BicycleCount = 0
            };

            return GenericResult<BikeTypeDto>.Success(resultDto, "Bike type created successfully");
        }

        public async Task<GenericResult<BikeTypeDto>> UpdateBikeTypeAsync(int typeId, UpdateBikeTypeDto dto)
        {
            // Ki?m tra bike type có t?n t?i không
            var existingBikeType = await _repository.GetByIdAsync(typeId);
            if (existingBikeType == null)
            {
                return GenericResult<BikeTypeDto>.Failure("Bike type not found");
            }

            // Validate tên không ðý?c r?ng
            if (string.IsNullOrWhiteSpace(dto.TypeName))
            {
                return GenericResult<BikeTypeDto>.Failure("Bike type name is required");
            }

            // Ki?m tra tên m?i có trùng v?i tên khác không (lo?i tr? ID hi?n t?i)
            var isNameExist = await _repository.IsTypeNameExistAsync(dto.TypeName, typeId);
            if (isNameExist)
            {
                return GenericResult<BikeTypeDto>.Failure("Bike type name already exists");
            }

            // C?p nh?t
            existingBikeType.TypeName = dto.TypeName.Trim();
            await _repository.UpdateAsync(existingBikeType);

            var resultDto = new BikeTypeDto
            {
                TypeId = existingBikeType.TypeId,
                TypeName = existingBikeType.TypeName,
                BicycleCount = existingBikeType.Bicycles.Count
            };

            return GenericResult<BikeTypeDto>.Success(resultDto, "Bike type updated successfully");
        }

        public async Task<GenericResult<bool>> DeleteBikeTypeAsync(int typeId)
        {
            // Ki?m tra bike type có t?n t?i không
            var exists = await _repository.ExistsAsync(typeId);
            if (!exists)
            {
                return GenericResult<bool>.Failure("Bike type not found");
            }

            // Ki?m tra có bicycle nào ðang s? d?ng lo?i này không
            var bicycleCount = await _repository.CountBicyclesByTypeAsync(typeId);
            if (bicycleCount > 0)
            {
                return GenericResult<bool>.Failure($"Cannot delete bike type. There are {bicycleCount} bicycle(s) using this type");
            }

            await _repository.DeleteAsync(typeId);

            return GenericResult<bool>.Success(true, "Bike type deleted successfully");
        }
    }
}
