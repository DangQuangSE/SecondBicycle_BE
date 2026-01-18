using Application.DTOs.UserProfile;
using Application.Helpers;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repo;
        private readonly IStorageService _storageService; 

        public UserProfileService(IUserProfileRepository repo, IStorageService storageService)
        {
            _repo = repo;
            _storageService = storageService;
        }
        public async Task<GenericResult<List<UserProfileDto>>> GetAllProfilesAsync()
        {
            var profiles = await _repo.GetAllAsync();

            var resultDtos = profiles.Select(p => new UserProfileDto
            {
                ProfileId = p.ProfileId,
                UserId = p.UserId,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                AvatarUrl = p.AvatarUrl
            }).ToList();

            return GenericResult<List<UserProfileDto>>.Success(resultDtos);
        }

        public async Task<GenericResult<UserProfileDto>> GetProfileByUserIdAsync(int userId)
        {
            var profile = await _repo.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return GenericResult<UserProfileDto>.Failure("Profile not found");
            }

            // Map sang DTO
            var dto = new UserProfileDto
            {
                ProfileId = profile.ProfileId,
                UserId = profile.UserId,
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                AvatarUrl = profile.AvatarUrl
            };

            return GenericResult<UserProfileDto>.Success(dto);
        }

        public async Task<GenericResult<UserProfileDto>> CreateProfileAsync(CreateUserProfileDto request)
        {
            // Kiểm tra xem User này đã có Profile chưa (vì quan hệ 1-1)
            var existingProfile = await _repo.GetByUserIdAsync(request.UserId);
            if (existingProfile != null)
            {
                return GenericResult<UserProfileDto>.Failure("User already has a profile.");
            }

            // Map DTO sang Entity
            var newProfile = new UserProfile
            {
                UserId = request.UserId,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                AvatarUrl = null // Mặc định chưa có avatar
            };

            await _repo.AddAsync(newProfile);

            // Map ngược lại DTO để trả về kết quả
            var resultDto = new UserProfileDto
            {
                ProfileId = newProfile.ProfileId,
                UserId = newProfile.UserId,
                FullName = newProfile.FullName,
                PhoneNumber = newProfile.PhoneNumber,
                Address = newProfile.Address,
                AvatarUrl = newProfile.AvatarUrl
            };

            return GenericResult<UserProfileDto>.Success(resultDto, "Profile created successfully");
        }
        public async Task<GenericResult<bool>> UpdateBasicInfoAsync(int userId, UpdateUserProfileDto request)
        {
            var profile = await _repo.GetByUserIdAsync(userId);
            if (profile == null)
            {
                // Gọi hàm Failure từ class của bạn
                return GenericResult<bool>.Failure("Profile not found");
            }

            profile.FullName = request.FullName ?? profile.FullName;
            profile.PhoneNumber = request.PhoneNumber ?? profile.PhoneNumber;
            profile.Address = request.Address ?? profile.Address;

            await _repo.UpdateAsync(profile);

            // Gọi hàm Success, data là true
            return GenericResult<bool>.Success(true, "Info updated successfully");
        }

        public async Task<GenericResult<string>> UpdateAvatarAsync(int userId, UpdateAvatarDto request)
        {
            var profile = await _repo.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return GenericResult<string>.Failure("Profile not found");
            }

            if (request.AvatarFile == null || request.AvatarFile.Length == 0)
            {
                return GenericResult<string>.Failure("Invalid file");
            }

            // BƯỚC 1: Gọi Service để lưu file. 
            // Service này đã trả về đường dẫn đầy đủ (ví dụ: "/uploads/avatars/abc.jpg")
            var avatarUrl = await _storageService.SaveFileAsync(request.AvatarFile, "avatars"); // Lưu ý: tham số folderName chỉ cần tên folder, không cần "uploads/" nếu bên trong service đã xử lý

            // Kiểm tra nếu lưu lỗi
            if (string.IsNullOrEmpty(avatarUrl))
            {
                return GenericResult<string>.Failure("Failed to upload avatar");
            }     

            // BƯỚC 2: Lưu đường dẫn vào DB
            profile.AvatarUrl = avatarUrl;
            await _repo.UpdateAsync(profile);

            // Trả về URL để Frontend hiển thị
            return GenericResult<string>.Success(avatarUrl, "Avatar updated successfully");
        }
        public async Task<GenericResult<bool>> DeleteProfileAsync(int userId)
        {
            var profile = await _repo.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return GenericResult<bool>.Failure("Profile not found");
            }

            await _repo.DeleteAsync(profile);

            return GenericResult<bool>.Success(true, "Profile deleted successfully");
        }
    }
}
