using Application.DTOs.Auth;
using Application.Helpers;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthService(IAuthRepository authRepository, IGoogleAuthService googleAuthService)
        {
            _authRepository = authRepository;
            _googleAuthService = googleAuthService;
        }

        public async Task<GenericResult<AuthResponse>> LoginWithGoogleAsync(string idToken)
        {
            try
            {
                var googleUserInfo = await _googleAuthService.ValidateGoogleTokenAsync(idToken);
                
                if (googleUserInfo == null)
                {
                    return GenericResult<AuthResponse>.Failure("Invalid Google Token");
                }

                var user = await _authRepository.GetUserByEmailAsync(googleUserInfo.Email);

                if (user == null)
                {
                    var baseUsername = googleUserInfo.GivenName ?? googleUserInfo.Email.Split('@')[0];
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
                        Email = googleUserInfo.Email,
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

                var token = JwtHelper.CreateToken(user);
                var refreshToken = RefreshTokenHelper.GenerateRefreshToken();

                return GenericResult<AuthResponse>.Success(new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return GenericResult<AuthResponse>.Failure($"Login failed: {ex.Message}");
            }
        }
    }
}
