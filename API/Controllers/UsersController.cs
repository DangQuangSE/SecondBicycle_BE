using Application.DTOs.Users;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    //[Authorize(Roles = "Admin")]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            await _service.CreateAsync(dto);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent(); // 204
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Ban(int id)
        {
            await _service.BanAsync(id);
            return NoContent(); // 204
        }
    }

}
