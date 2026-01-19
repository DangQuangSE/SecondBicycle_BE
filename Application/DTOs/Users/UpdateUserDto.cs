
namespace Application.DTOs.Users
{
    public class UpdateUserDto
    {
        public int RoleId { get; set; }
        public byte Status { get; set; } // 0 = Banned, 1 = Active
    }

}
