//using Microsoft.AspNetCore.Mvc;
//using ShoeCartBackend.Common;
//using ShoeCartBackend.DTOs.AuthDTO;
//using ShoeCartBackend.Services.Interfaces;
//using System.Threading.Tasks;

//namespace ShoeCartBackend.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthService _authService;

//        public AuthController(IAuthService authService)
//        {
//            _authService = authService;
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> AddUser([FromForm] RegisterRequestDto registerRequestDto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var response = await _authService.RegisterAsync(registerRequestDto);

//            if (response.StatusCode == 200)
//                return Ok(response);
//            else if (response.StatusCode == 409)
//                return StatusCode(StatusCodes.Status409Conflict, response.Message);
//            else
//                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromForm] LoginRequestDto loginRequestDto)
//        {
//            var result = await _authService.LoginAsync(loginRequestDto);

//            var response = new ApiResponse<object>
//            {
//                StatusCode = result.StatusCode,
//                Message = result.Message,
//                Data = new
//                {
//                    AccessToken = result.AccessToken,
//                    RefreshToken = result.RefreshToken
//                }
//            };

//            return StatusCode(result.StatusCode, response);
//        }

//        // New Refresh Token endpoint
//        [HttpPost("refresh-token")]
//        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
//        {
//            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
//                return BadRequest(new { Message = "Refresh token is required" });

//            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

//            var response = new ApiResponse<object>
//            {
//                StatusCode = result.StatusCode,
//                Message = result.Message,
//                Data = new
//                {
//                    AccessToken = result.AccessToken,
//                    RefreshToken = result.RefreshToken
//                }
//            };

//            return StatusCode(result.StatusCode, response);
//        }
//    }
//}



using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs.AuthDTO;
using ShoeCartBackend.Services.Interfaces;
using System;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result.StatusCode != 200) return StatusCode(result.StatusCode, result);

            // Set cookies
            SetTokenCookies(result.AccessToken, result.RefreshToken);

            return Ok(new
            {
                message = result.Message,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Refresh token missing" });

            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result.StatusCode != 200) return Unauthorized(result);

            SetTokenCookies(result.AccessToken, result.RefreshToken);

            return Ok(new
            {
                message = result.Message,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Refresh token missing" });

            var success = await _authService.RevokeTokenAsync(refreshToken);
            if (!success) return BadRequest(new { message = "Invalid token" });

            DeleteTokenCookies();
            return Ok(new { message = "Logged out successfully" });
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }

        private void DeleteTokenCookies()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
        }
    }
}
