using Application.DTOs.Auth;
using Application.Helpers;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<GenericResult<AuthResponse>> LoginWithGoogleAsync(string idToken);
    }
}
