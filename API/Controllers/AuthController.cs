using Application.DTOs.Auth;
using Application.Helpers;
using Application.IServices;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService) : ControllerBase
    {
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
