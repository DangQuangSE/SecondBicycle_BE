using Application.DTOs.Auth;
using Application.IServices;
using Google.Apis.Auth;

namespace Infrastructure.ExternalServices
{
    public class GoogleAuthService : IGoogleAuthService
    {
        public async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    return null;
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                if (payload == null)
                {
                    return null;
                }

                return new GoogleUserInfo
                {
                    Email = payload.Email,
                    GivenName = payload.GivenName,
                    FamilyName = payload.FamilyName,
                    Name = payload.Name
                };
            }
            catch (InvalidJwtException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
