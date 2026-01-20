namespace Application.IServices
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string email, string username, int roleId, string? roleName);
    }
}
