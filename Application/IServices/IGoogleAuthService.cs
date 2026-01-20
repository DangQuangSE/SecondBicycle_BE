using Application.DTOs.Auth;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken);
    }
}
