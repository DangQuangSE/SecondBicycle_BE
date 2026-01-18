using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserProfile
{
    public class UpdateAvatarDto
    {
        public IFormFile AvatarFile { get; set; } = null!;
    }
}
