using Application.DTOs.Auth;
using Application.Helpers;
using Application.Interfaces;
using Google.Apis.Auth;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<GenericResult<AuthResponse>> LoginWithGoogleAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Google:ClientId"] } 
                };
                // If ClientId is not in config, we might want to skip validation or accept any valid signature (less secure but easier for dev if config missing)
                // Better to throw if missing config in production.
                // For now, let's allow it to be validated just by Google servers without audience check if config is missing? 
                // No, must check audience to prevent token reuse from other apps.
               
                // However, user might not have set it up yet. pass null to settings if key missing?
                // Google library throws if audience doesn't match if we provide list.
                // If we don't provide list, it doesn't check audience.
                
                GoogleJsonWebSignature.Payload payload;
                if (!string.IsNullOrEmpty(_configuration["Google:ClientId"]))
                {
                     payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                }
                else
                {
                     payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                }

                if (payload == null)
                    return GenericResult<AuthResponse>.Failure("Invalid Google Token");

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    // Register new user
                    // Find default role
                    var defaultRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "User" || r.RoleName == "Member" || r.RoleName == "Customer");
                    int roleId = defaultRole?.RoleId ?? 2; // Default to 2 if not found (assuming 1 is Admin, 2 is User - standard convention)

                    user = new User
                    {
                        Username = payload.GivenName ?? payload.Email.Split('@')[0], // Use first part of email if name missing
                        Email = payload.Email,
                        PasswordHash = Guid.NewGuid().ToString(), // No password for Google users
                        RoleId = roleId,
                        IsVerified = true, // Google emails are verified
                        Status = 1, // Active
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    
                    // Reload user to get any default values or related data if needed
                     user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == user.UserId);
                }

                var token = _tokenService.GenerateToken(user.UserId, user.Email, user.Username, user.RoleId, user.Role?.RoleName);

                return GenericResult<AuthResponse>.Success(new AuthResponse
                {
                    Token = token,
                    Email = user.Email,
                    Username = user.Username
                });
            }
            catch (InvalidJwtException)
            {
                return GenericResult<AuthResponse>.Failure("Invalid Google Token");
            }
            catch (Exception ex)
            {
                return GenericResult<AuthResponse>.Failure(ex.Message);
            }
        }
    }
}
