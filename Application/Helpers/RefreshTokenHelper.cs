using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class RefreshTokenHelper
    {
        private static readonly string JwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
        public static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(randomBytes);
            return token;
        }

        public static string HashRefreshToken(string refreshTokenBase64)
        {
            //chuyển key từ string sang byte[]
            byte[] keyBytes = Encoding.UTF8.GetBytes(JwtKey);
            //chuyển refreshToken từ base64 string sang byte[]
            byte[] tokenBytes = Convert.FromBase64String(refreshTokenBase64);

            using var hmac = new HMACSHA512(keyBytes);
            //hash byte của token với key
            byte[] hashTokenBytes = hmac.ComputeHash(tokenBytes);
            //chuyển hash byte sang base64 string để lưu vào db
            return Convert.ToBase64String(hashTokenBytes);
        }

        public static bool VerifyRefreshToken(string refreshTokenBase64, string storedHashedToken)
        {
            string hashedToken = HashRefreshToken(refreshTokenBase64);

            byte[] a = Convert.FromBase64String(hashedToken);
            byte[] b = Convert.FromBase64String(storedHashedToken);

            //constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
