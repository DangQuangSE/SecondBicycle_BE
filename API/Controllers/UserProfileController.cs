using Application.DTOs.UserProfile;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _service;

        public UserProfileController(IUserProfileService service)
        {
            _service = service;
        }

        // Helper function để lấy UserId từ JWT Token hiện tại
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid Token");
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _service.GetAllProfilesAsync();
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfileByUserId(int userId)
        {
            var result = await _service.GetProfileByUserIdAsync(userId);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.GetProfileByUserIdAsync(userId);
                if (!result.IsSuccess) return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateUserProfileDto request)
        {
            // Lưu ý: Bạn có thể lấy UserId từ Token nếu muốn bảo mật hơn, 
            // thay vì cho phép client gửi UserId tùy ý trong body.
            // Ví dụ: request.UserId = GetCurrentUserId();

            var result = await _service.CreateProfileAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            // Trả về 201 Created
            return StatusCode(201, result);
        }

        [HttpPut("update-info")]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateUserProfileDto request)
        {
            var userId = GetCurrentUserId();
            // result lúc này là GenericResult<bool>
            var result = await _service.UpdateBasicInfoAsync(userId, request);

            // Kiểm tra IsSuccess (theo property class của bạn)
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("update-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateAvatarDto request)
        {
            var userId = GetCurrentUserId();
            // result lúc này là GenericResult<string>
            var result = await _service.UpdateAvatarAsync(userId, request);

            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            // Logic phụ: Nếu user thường tự xóa, cần check xem userId trên URL có khớp với Token không
            // var currentUserId = GetCurrentUserId();
            // if (userId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            var result = await _service.DeleteProfileAsync(userId);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
