using Application.DTOs.Brand;
using Application.Helpers;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(GenericResult<List<BrandViewDto>>.Success(
                result,
                "Retrieved all brands successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(GenericResult<BrandViewDto>.Success(
                result,
                $"Retrieved brand with ID {id} successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.BrandId },
                GenericResult<BrandViewDto>.Success(
                    result,
                    "Brand created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(GenericResult<BrandViewDto>.Success(
                result,
                $"Brand with ID {id} updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(GenericResult<object>.Success(
                null,
                $"Brand with ID {id} deleted successfully"));
        }
    }
}
