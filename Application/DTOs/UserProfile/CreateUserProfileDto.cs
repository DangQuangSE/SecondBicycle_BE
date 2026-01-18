using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserProfile
{
    public class CreateUserProfileDto
    {
        public int UserId { get; set; } // Bắt buộc phải có để liên kết
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
