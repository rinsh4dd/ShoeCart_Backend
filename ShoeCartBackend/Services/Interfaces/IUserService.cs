using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs; // If you want to create DTOs
using ShoeCartBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<User>>> GetAllUsersAsync();
        Task<ApiResponse<User>> GetUserByIdAsync(int id);

        Task<ApiResponse<string>> BlockUnblockUserAsync(int id);
        Task<ApiResponse<string>> SoftDeleteUserAsync(int id);
    }
}
