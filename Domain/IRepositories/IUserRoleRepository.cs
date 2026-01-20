using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        //get user role by name
        Task<UserRole?> GetByNameAsync(string roleName);
    }
}
