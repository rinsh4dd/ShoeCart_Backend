using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Services.Interfaces;
using System.Threading.Tasks;

namespace ShoeCartBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("block-unblock/{id}")]
        public async Task<IActionResult> BlockUnblock(int id)
        {
            var response = await _userService.BlockUnblockUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _userService.SoftDeleteUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
