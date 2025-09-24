using System.Threading.Tasks;
using ShoeCartBackend.DTOs.AuthDTO;
using ShoeCartBackend.Models;

namespace ShoeCartBackend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        //Task<string> LoginAsync(string email, string password);
        //Task<bool> IsEmailExistsAsync(string email);
        //Task<User?> GetUserByEmailAsync(string email);

    }
}
