using Application.DTOs.Auth;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IAuthService
    {
        Task<GenericResult<LoginResponse>> Login(LoginRequest request);
        Task<GenericResult<string>> Register(RegisterRequest request);
    }
}
