using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<TestController> _logger;

    public TestController(AppDbContext context, ILogger<TestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    [HttpGet("database-connection")]
    public async Task<IActionResult> TestDatabaseConnection()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (canConnect)
            {
                var userCount = await _context.Users.CountAsync();
                var bicycleCount = await _context.Bicycles.CountAsync();
                
                return Ok(new
                {
                    success = true,
                    message = "K?t n?i database thành công!",
                    server = "20.6.88.113",
                    database = "SP26SWP06_BikeTrading",
                    statistics = new
                    {
                        totalUsers = userCount,
                        totalBicycles = bicycleCount
                    }
                });
            }

            return StatusCode(500, new
            {
                success = false,
                message = "Không th? k?t n?i ð?n database"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi ki?m tra k?t n?i database");
            return StatusCode(500, new
            {
                success = false,
                message = "L?i k?t n?i database",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.UserProfile)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.IsVerified,
                    u.Status,
                    u.CreatedAt,
                    Role = u.Role.RoleName,
                    Profile = u.UserProfile != null ? new
                    {
                        u.UserProfile.FullName,
                        u.UserProfile.PhoneNumber,
                        u.UserProfile.Address
                    } : null
                })
                .Take(10)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = users.Count,
                data = users
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi l?y danh sách users");
            return StatusCode(500, new
            {
                success = false,
                message = "L?i khi l?y d? li?u",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all bicycles
    /// </summary>
    [HttpGet("bicycles")]
    public async Task<IActionResult> GetBicycles()
    {
        try
        {
            var bicycles = await _context.Bicycles
                .Include(b => b.Brand)
                .Include(b => b.Type)
                .Include(b => b.BicycleDetail)
                .Select(b => new
                {
                    b.BikeId,
                    b.ModelName,
                    b.SerialNumber,
                    b.Color,
                    b.Condition,
                    Brand = b.Brand != null ? b.Brand.BrandName : null,
                    Type = b.Type != null ? b.Type.TypeName : null,
                    Detail = b.BicycleDetail != null ? new
                    {
                        b.BicycleDetail.FrameSize,
                        b.BicycleDetail.FrameMaterial,
                        b.BicycleDetail.WheelSize,
                        b.BicycleDetail.BrakeType
                    } : null
                })
                .Take(10)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = bicycles.Count,
                data = bicycles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi l?y danh sách bicycles");
            return StatusCode(500, new
            {
                success = false,
                message = "L?i khi l?y d? li?u",
                error = ex.Message
            });
        }
    }
}
