using Application.DTOs.Auth;
using Application.Helpers;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, ITokenService tokenService, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<GenericResult<AuthResponse>> LoginWithGoogleAsync(string idToken)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    return GenericResult<AuthResponse>.Failure("Google ClientId is not configured.");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                if (payload == null)
                {
                    return GenericResult<AuthResponse>.Failure("Invalid Google Token");
                }

                var user = await _authRepository.GetUserByEmailAsync(payload.Email);

                if (user == null)
                {
                    var baseUsername = payload.GivenName ?? payload.Email.Split('@')[0];
                    var username = baseUsername;
                    int suffix = 1;
                    while (await _authRepository.UsernameExistsAsync(username))
                    {
                        username = $"{baseUsername}{suffix}";
                        suffix++;
                    }

                    user = new User
                    {
                        Username = username,
                        Email = payload.Email,
                        PasswordHash = Guid.NewGuid().ToString(),
                        RoleId = 2,
                        IsVerified = true,
                        Status = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _authRepository.AddUserAsync(user);
                    await _authRepository.SaveChangesAsync();

                    user = await _authRepository.GetUserWithRoleByIdAsync(user.UserId);
                    
                    if (user == null)
                    {
                        return GenericResult<AuthResponse>.Failure("Failed to create user.");
                    }
                }

                var token = _tokenService.GenerateToken(
                    user.UserId, 
                    user.Email ?? string.Empty, 
                    user.Username ?? string.Empty, 
                    user.RoleId, 
                    user.Role?.RoleName);

                return GenericResult<AuthResponse>.Success(new AuthResponse
                {
                    Token = token,
                    Email = user.Email ?? string.Empty,
                    Username = user.Username ?? string.Empty
                });
            }
            catch (InvalidJwtException)
            {
                return GenericResult<AuthResponse>.Failure("Invalid Google Token");
            }
            catch (Exception ex)
            {
                return GenericResult<AuthResponse>.Failure($"Login failed: {ex.Message}");
            }
        }
    }
}
