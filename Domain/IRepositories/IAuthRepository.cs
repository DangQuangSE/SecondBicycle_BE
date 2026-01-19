using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserWithRoleByIdAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<UserRole?> GetRoleForNewUserAsync();
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
    }
}
