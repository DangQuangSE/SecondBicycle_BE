using Application.DTOs.Auth;
using Application.Interfaces;
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
            if (request == null || string.IsNullOrEmpty(request.IdToken))
                return BadRequest("Token is required");

            var result = await _authService.LoginWithGoogleAsync(request.IdToken);

            // Handle errors generically
            if (!result.IsSuccess)
            {
                // You might want to return 400 or 401 based on message, 
                // but usually failures in auth are 400 (Bad Request) or 401 (Unauthorized)
                return BadRequest(new { message = result.Message ?? "Login failed", errors = result.Errors });
            }

            return Ok(result.Data);
        }
    }
}
