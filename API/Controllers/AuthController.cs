using Application.DTOs.Auth;
using Application.Helpers;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
            {
                return BadRequest(GenericResult<AuthResponse>.Failure("IdToken is required"));
            }

            var result = await _authService.LoginWithGoogleAsync(request.IdToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }
    }
}
