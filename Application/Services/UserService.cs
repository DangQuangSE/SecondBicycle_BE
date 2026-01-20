using Application.DTOs.Users;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;


namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();

            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,

                IsVerified = u.IsVerified ?? false,
                Status = u.Status ?? 1,

                RoleName = u.Role.RoleName
            }).ToList();
        }


        public async Task<UserResponseDto> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("User not found");

            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,

                IsVerified = user.IsVerified ?? false,
                Status = user.Status ?? 1,

                RoleName = user.Role.RoleName
            };

        }

        public async Task CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                RoleId = dto.RoleId,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = "",    
                IsVerified = false,
                Status = 1,         
                CreatedAt = DateTime.Now
            };


            await _repo.AddAsync(user);
        }

        public async Task UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("User not found");


            user.RoleId = dto.RoleId;
            user.Status = dto.Status;

            await _repo.UpdateAsync(user);
        }

        public async Task BanAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("User not found");


            user.Status = 0; 
            await _repo.UpdateAsync(user);
        }
    }

}
