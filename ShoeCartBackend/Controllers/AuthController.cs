using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.DTOs.AuthDTO;
using ShoeCartBackend.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeCartBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // -------------------- REGISTER --------------------
        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] RegisterRequestDto registerRequestDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

             var Response = await _authService.RegisterAsync(registerRequestDto);

            if (Response.StatusCode == 200)
            {
                return Ok(Response);
            }
            else if(Response.StatusCode == 409)
            {
                return StatusCode(StatusCodes.Status409Conflict, Response.Message);
            }
            else if (Response.StatusCode == 500)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,Response.Message);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var Result = await _authService.LoginAsync(loginRequestDto);
            return StatusCode(Result.StatusCode, Result);
        }
        
    }
}
