//using System.Threading.Tasks;
//using ShoeCartBackend.DTOs.AuthDTO;
//using ShoeCartBackend.Models;

//namespace ShoeCartBackend.Services.Interfaces
//{
//    public interface IAuthService
//    {
//        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
//        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto);

//        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
//    }
//}


using System.Threading.Tasks;
using ShoeCartBackend.DTOs.AuthDTO;
using ShoeCartBackend.Models;

namespace ShoeCartBackend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken); 
    }
}
