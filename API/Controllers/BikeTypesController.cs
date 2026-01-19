using Application.DTOs.BikeType;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeTypesController : ControllerBase
    {
        private readonly IBikeTypeService _service;

        public BikeTypesController(IBikeTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// L?y t?t c? lo?i xe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBikeTypes()
        {
            var result = await _service.GetAllBikeTypesAsync();
            return Ok(result);
        }

        /// <summary>
        /// L?y lo?i xe theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBikeTypeById(int id)
        {
            var result = await _service.GetBikeTypeByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// T?o m?i lo?i xe
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBikeType([FromBody] CreateBikeTypeDto dto)
        {
            var result = await _service.CreateBikeTypeAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            // Tr? v? 201 Created v?i Location header
            return CreatedAtAction(
                nameof(GetBikeTypeById),
                new { id = result.Data!.TypeId },
                result
            );
        }

        /// <summary>
        /// C?p nh?t lo?i xe
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBikeType(int id, [FromBody] UpdateBikeTypeDto dto)
        {
            var result = await _service.UpdateBikeTypeAsync(id, dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa lo?i xe
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBikeType(int id)
        {
            var result = await _service.DeleteBikeTypeAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return NoContent(); // 204 No Content
        }
    }
}
