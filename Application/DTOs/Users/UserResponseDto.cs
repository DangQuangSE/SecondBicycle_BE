
namespace Application.DTOs.Users
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsVerified { get; set; }
        public byte Status { get; set; }
    }

}
