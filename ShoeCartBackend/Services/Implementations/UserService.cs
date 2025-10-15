using ShoeCartBackend.Common;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using ShoeCartBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _genericRepo;
        private readonly IUserRepository _userRepo;
        public UserService(IGenericRepository<User> genericRepo, IUserRepository userRepo)
        {
            _genericRepo = genericRepo;
            _userRepo = userRepo;
        }
        public async Task<ApiResponse<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _genericRepo.GetAllAsync();
            return new ApiResponse<IEnumerable<User>>(200, "Users retrieved successfully", users);
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            var user = await _genericRepo.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return new ApiResponse<User>(404, "User not found");

            return new ApiResponse<User>(200, "User retrieved successfully", user);
        }
        public async Task<ApiResponse<string>> BlockUnblockUserAsync(int id)
        {
            var user = await _genericRepo.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return new ApiResponse<string>(404, "User not found");
            if(user.Role == Roles.admin)
            {
                return new ApiResponse<string>(403, "Action forbidden. Admin users cannot be modified.");
            }
            await _userRepo.BlockUnblockUserAsync(id);
            return new ApiResponse<string>(200, $"User {(user.IsBlocked ? "unblocked" : "blocked")} successfully");
        }
        public async Task<ApiResponse<string>> SoftDeleteUserAsync(int id)
        {
            var user = await _genericRepo.GetByIdAsync(id);
            if (user == null || user.IsDeleted )
                return new ApiResponse<string>(404, "User not found");
            if(user.Role == Roles.admin)
            {
                return new ApiResponse<string>(403, "Action forbidden. Admin users cannot be modified.");
            }
            await _userRepo.SoftDeleteUserAsync(id);
            return new ApiResponse<string>(200, "User Soft-Deleted successfully");
        }
    }
}
