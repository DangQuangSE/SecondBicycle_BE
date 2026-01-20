
using Application.Services;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("/login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await userService.Login(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("/register")]
        public async Task<ActionResult<GenericResult<string>>> Register([FromBody] RegisterRequest request)
        {
            var response = await userService.Register(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
