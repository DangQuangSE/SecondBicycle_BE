using Application.DTOs.BikeType;
using Application.Helpers;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BikeTypeService : IBikeTypeService
    {
        private readonly IBikeTypeRepository _repository;
        private readonly ILogger<BikeTypeService> _logger;

        public BikeTypeService(IBikeTypeRepository repository, ILogger<BikeTypeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GenericResult<List<BikeTypeDto>>> GetAllBikeTypesAsync()
        {
            try
            {
                var bikeTypes = await _repository.GetAllAsync();

                var bikeTypeDtos = bikeTypes.Select(bt => new BikeTypeDto
                {
                    TypeId = bt.TypeId,
                    TypeName = bt.TypeName,
                    BicycleCount = bt.Bicycles?.Count ?? 0
                }).ToList();

                return GenericResult<List<BikeTypeDto>>.Success(bikeTypeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all bike types");
                return GenericResult<List<BikeTypeDto>>.Failure("An error occurred while retrieving bike types");
            }
        }

        public async Task<GenericResult<BikeTypeDto>> GetBikeTypeByIdAsync(int typeId)
        {
            try
            {
                // Validate ID
                if (typeId <= 0)
                {
                    return GenericResult<BikeTypeDto>.Failure("Invalid bike type ID");
                }

                var bikeType = await _repository.GetByIdAsync(typeId);
                
                if (bikeType == null)
                {
                    _logger.LogWarning("Bike type with ID {TypeId} not found", typeId);
                    return GenericResult<BikeTypeDto>.Failure("Bike type not found");
                }

                var bikeTypeDto = new BikeTypeDto
                {
                    TypeId = bikeType.TypeId,
                    TypeName = bikeType.TypeName,
                    BicycleCount = bikeType.Bicycles?.Count ?? 0
                };

                return GenericResult<BikeTypeDto>.Success(bikeTypeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting bike type with ID {TypeId}", typeId);
                return GenericResult<BikeTypeDto>.Failure("An error occurred while retrieving bike type");
            }
        }

        public async Task<GenericResult<BikeTypeDto>> CreateBikeTypeAsync(CreateBikeTypeDto dto)
        {
            try
            {
                // Validate input
                var validationErrors = ValidateTypeName(dto.TypeName);
                if (validationErrors.Any())
                {
                    return GenericResult<BikeTypeDto>.Failure(validationErrors.ToArray());
                }

                // Ki?m tra tên ð? t?n t?i chýa
                var isExist = await _repository.IsTypeNameExistAsync(dto.TypeName);
                if (isExist)
                {
                    _logger.LogWarning("Attempt to create bike type with existing name: {TypeName}", dto.TypeName);
                    return GenericResult<BikeTypeDto>.Failure("Bike type name already exists");
                }

                // T?o entity m?i
                var newBikeType = new BikeType
                {
                    TypeName = dto.TypeName.Trim()
                };

                var createdBikeType = await _repository.AddAsync(newBikeType);

                _logger.LogInformation("Bike type created successfully with ID {TypeId}", createdBikeType.TypeId);

                var resultDto = new BikeTypeDto
                {
                    TypeId = createdBikeType.TypeId,
                    TypeName = createdBikeType.TypeName,
                    BicycleCount = 0
                };

                return GenericResult<BikeTypeDto>.Success(resultDto, "Bike type created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating bike type");
                return GenericResult<BikeTypeDto>.Failure("An error occurred while creating bike type");
            }
        }

        public async Task<GenericResult<BikeTypeDto>> UpdateBikeTypeAsync(int typeId, UpdateBikeTypeDto dto)
        {
            try
            {
                // Validate ID
                if (typeId <= 0)
                {
                    return GenericResult<BikeTypeDto>.Failure("Invalid bike type ID");
                }

                // Validate input
                var validationErrors = ValidateTypeName(dto.TypeName);
                if (validationErrors.Any())
                {
                    return GenericResult<BikeTypeDto>.Failure(validationErrors.ToArray());
                }

                // Ki?m tra bike type có t?n t?i không
                var existingBikeType = await _repository.GetByIdAsync(typeId);
                if (existingBikeType == null)
                {
                    _logger.LogWarning("Attempt to update non-existent bike type with ID {TypeId}", typeId);
                    return GenericResult<BikeTypeDto>.Failure("Bike type not found");
                }

                // Ki?m tra tên m?i có trùng v?i tên khác không (lo?i tr? ID hi?n t?i)
                var isNameExist = await _repository.IsTypeNameExistAsync(dto.TypeName, typeId);
                if (isNameExist)
                {
                    _logger.LogWarning("Attempt to update bike type {TypeId} with existing name: {TypeName}", typeId, dto.TypeName);
                    return GenericResult<BikeTypeDto>.Failure("Bike type name already exists");
                }

                // C?p nh?t
                existingBikeType.TypeName = dto.TypeName.Trim();
                await _repository.UpdateAsync(existingBikeType);

                _logger.LogInformation("Bike type {TypeId} updated successfully", typeId);

                var resultDto = new BikeTypeDto
                {
                    TypeId = existingBikeType.TypeId,
                    TypeName = existingBikeType.TypeName,
                    BicycleCount = existingBikeType.Bicycles?.Count ?? 0
                };

                return GenericResult<BikeTypeDto>.Success(resultDto, "Bike type updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating bike type with ID {TypeId}", typeId);
                return GenericResult<BikeTypeDto>.Failure("An error occurred while updating bike type");
            }
        }

        public async Task<GenericResult<bool>> DeleteBikeTypeAsync(int typeId)
        {
            try
            {
                // Validate ID
                if (typeId <= 0)
                {
                    return GenericResult<bool>.Failure("Invalid bike type ID");
                }

                // Ki?m tra bike type có t?n t?i không
                var exists = await _repository.ExistsAsync(typeId);
                if (!exists)
                {
                    _logger.LogWarning("Attempt to delete non-existent bike type with ID {TypeId}", typeId);
                    return GenericResult<bool>.Failure("Bike type not found");
                }

                // Ki?m tra có bicycle nào ðang s? d?ng lo?i này không
                var bicycleCount = await _repository.CountBicyclesByTypeAsync(typeId);
                if (bicycleCount > 0)
                {
                    _logger.LogWarning("Attempt to delete bike type {TypeId} with {Count} bicycles using it", typeId, bicycleCount);
                    return GenericResult<bool>.Failure($"Cannot delete bike type. There are {bicycleCount} bicycle(s) using this type");
                }

                await _repository.DeleteAsync(typeId);

                _logger.LogInformation("Bike type {TypeId} deleted successfully", typeId);

                return GenericResult<bool>.Success(true, "Bike type deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting bike type with ID {TypeId}", typeId);
                return GenericResult<bool>.Failure("An error occurred while deleting bike type");
            }
        }

        /// <summary>
        /// Validate TypeName v?i các rule c? th?
        /// </summary>
        private List<string> ValidateTypeName(string typeName)
        {
            var errors = new List<string>();

            // Ki?m tra null ho?c empty
            if (string.IsNullOrWhiteSpace(typeName))
            {
                errors.Add("Bike type name is required");
                return errors;
            }

            var trimmedName = typeName.Trim();

            // Ki?m tra ð? dài t?i thi?u
            if (trimmedName.Length < 2)
            {
                errors.Add("Bike type name must be at least 2 characters long");
            }

            // Ki?m tra ð? dài t?i ða
            if (trimmedName.Length > 50)
            {
                errors.Add("Bike type name must not exceed 50 characters");
            }

            // Ki?m tra k? t? ð?c bi?t không h?p l? (ch? cho phép ch?, s?, d?u cách, d?u g?ch ngang)
            if (!Regex.IsMatch(trimmedName, @"^[a-zA-Z0-9\s\-àá???â?????Ã?????ÈÉ???Ê??????Í????Ó???Ô?????Õ?????ÙÚ???Ý??????????Ð]+$"))
            {
                errors.Add("Bike type name can only contain letters, numbers, spaces, and hyphens");
            }

            // Ki?m tra không ðý?c toàn d?u cách
            if (trimmedName.All(char.IsWhiteSpace))
            {
                errors.Add("Bike type name cannot be only whitespace");
            }

            return errors;
        }
    }
}
