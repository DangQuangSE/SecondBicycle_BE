using Domain.Entities;
namespace Domain.IRepositories
{
    public interface IAuthEmailRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
