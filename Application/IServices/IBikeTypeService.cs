using Application.DTOs.BikeType;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IBikeTypeService
    {
        Task<GenericResult<List<BikeTypeDto>>> GetAllBikeTypesAsync();
        Task<GenericResult<BikeTypeDto>> GetBikeTypeByIdAsync(int typeId);
        Task<GenericResult<BikeTypeDto>> CreateBikeTypeAsync(CreateBikeTypeDto dto);
        Task<GenericResult<BikeTypeDto>> UpdateBikeTypeAsync(int typeId, UpdateBikeTypeDto dto);
        Task<GenericResult<bool>> DeleteBikeTypeAsync(int typeId);
    }
}
