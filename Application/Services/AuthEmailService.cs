using Application.DTOs.Auth;
using Application.Helpers;
using Application.IServices;
using Domain.Entities;
using Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthEmailService(IUnitOfWork unitOfWork) : IAuthEmailService
    {
        public async Task<GenericResult<LoginResponse>> Login(LoginRequest request)
        {
            //check email exist
            var user = await unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user is null)
            {
                return GenericResult<LoginResponse>.Failure("Email does not exist");
            }
            //check password
            bool isPasswordValid = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return GenericResult<LoginResponse>.Failure("Invalid password");
            }

            //create access and refresh token
            var accessToken = JwtHelper.CreateToken(user);
            var refreshToken = RefreshTokenHelper.GenerateRefreshToken();

            await unitOfWork.BeginTransactionAsync();
            //hash refresh token and store to db
            var hashedRefreshToken = RefreshTokenHelper.HashRefreshToken(refreshToken);
            var refreshTokenEntity = new RefreshToken
            {
                Token = hashedRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };
            await unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);

            var rs = await unitOfWork.CommitAsync();
            if (rs <= 0)
                return GenericResult<LoginResponse>.Failure("login failed by saving refresh token to db.");

            return GenericResult<LoginResponse>.Success(new LoginResponse
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken
            }, "Login successful");
        }

        public async Task<GenericResult<string>> Register(RegisterRequest request)
        {
            //check xem  email exist
            var user = await unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user is not null)
            {
                return GenericResult<string>.Failure("Email already exists");
            }

            //kiem tra user role
            var role = await unitOfWork.UserRoles.GetByNameAsync(request.UserRole);
            if (role is null)
            {
                return GenericResult<string>.Failure("User role does not exist");
            }

            var hashedPassword = PasswordHasher.HashPassword(request.Password);
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Username = request.UserName,
                CreatedAt = DateTime.UtcNow,
                Role = role,
            };

            await unitOfWork.BeginTransactionAsync();
            await unitOfWork.Users.AddAsync(newUser);
            int rs = await unitOfWork.CommitAsync();

            if (rs > 0)
            {
                return GenericResult<string>.Success("", message: "Register successful");
            }
            else
            {
                return GenericResult<string>.Failure("Register failed");
            }
        }
    }
}
