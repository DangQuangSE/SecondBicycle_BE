
﻿using Domain.Entities;
namespace Domain.IRepositories
{
    public interface IAuthRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}