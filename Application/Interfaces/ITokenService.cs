using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string email, string username, int roleId, string? roleName);
    }
}
