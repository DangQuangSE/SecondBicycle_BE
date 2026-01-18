using Application.Interfaces;
using Infrastructure.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string email, string username, int roleId, string? roleName)
        {
            var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["JWT_KEY"];
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? _configuration["JWT_ISSUER"];
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? _configuration["JWT_AUDIENCE"];

            if (string.IsNullOrEmpty(key)) throw new Exception("JWT Key is missing");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, roleId.ToString()) 
            };
            
            if (!string.IsNullOrEmpty(roleName))
            {
                claims.Add(new Claim("RoleName", roleName));
            }

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
