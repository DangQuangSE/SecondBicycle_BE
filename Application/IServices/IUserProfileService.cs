using Application.DTOs.UserProfile;
using Application.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserProfileService
    {
        Task<GenericResult<List<UserProfileDto>>> GetAllProfilesAsync();
        Task<GenericResult<UserProfileDto>> GetProfileByUserIdAsync(int userId);
        Task<GenericResult<UserProfileDto>> CreateProfileAsync(CreateUserProfileDto request);
        Task<GenericResult<bool>> UpdateBasicInfoAsync(int userId, UpdateUserProfileDto request);
        Task<GenericResult<string>> UpdateAvatarAsync(int userId, UpdateAvatarDto request);
        Task<GenericResult<bool>> DeleteProfileAsync(int userId);
    }
}
