
namespace Application.DTOs.Users
{
    public class CreateUserDto
    {
        public int RoleId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

}
