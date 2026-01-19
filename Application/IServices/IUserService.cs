using Application.DTOs.Users;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateUserDto dto);
        Task UpdateAsync(int id, UpdateUserDto dto);
        Task BanAsync(int id);
    }

}
