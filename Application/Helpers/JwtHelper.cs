using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class JwtHelper
    {
        private static readonly string JwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
        private static readonly string JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        private static readonly string JwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
        private static readonly string JwtExpireMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES")!;

        public static string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                //new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(JwtExpireMinutes)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); ;
        }
    }
}
