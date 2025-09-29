using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using ShoeCartBackend.Services.Interfaces;
using ShoeCartBackend.DTOs.AuthDTO;
using ShoeCartBackend.Data;
using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Repositories.Implementations;

namespace ShoeCartBackend.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IConfiguration _configuration;
        public AuthService(AppDbContext appDbContext, IGenericRepository<User> userRepo,IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _userRepo = userRepo;
            _configuration = configuration;
        }


        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            try
            {
                if (registerRequestDto == null)
                {
                    throw new ArgumentNullException("Register request cannot be null");
                }
                registerRequestDto.Email = registerRequestDto.Email.ToLower().Trim();
                registerRequestDto.Name = registerRequestDto.Name.Trim();
                registerRequestDto.Password = registerRequestDto.Password.Trim();

                var UserExist = await _appDbContext.Users
                    .SingleOrDefaultAsync(u => u.Email == registerRequestDto.Email);

                if (UserExist != null)
                {
                    return new AuthResponseDto(409, "Email already exists");
                }
                var newUser = new User
                {
                    Email = registerRequestDto.Email,
                    Name = registerRequestDto.Name,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password),
                    Role = Roles.admin
                };
                await _userRepo.AddAsync(newUser);
                return new AuthResponseDto(200, "Registration Successfull");

            }
            catch (Exception ex)
            {
                return new AuthResponseDto(500, $"Error Adding User{ex.Message}");
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            try
            {
                if (loginRequestDto == null)
                {
                    throw new ArgumentException("Login request cannot be null");
                }
                loginRequestDto.Email = loginRequestDto.Email.Trim().ToLower();
                loginRequestDto.Password = loginRequestDto.Password.Trim();

                var user = await _appDbContext.Users
                    .SingleOrDefaultAsync(u => u.Email == loginRequestDto.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash))
                {
                    return new AuthResponseDto(401, "Invalid username or password");
                }
                else if (user.IsBlocked)
                {
                    return new AuthResponseDto(403, "This Account has been Blocked!");
                }
                var token = GenerateJwtToken(user);
                return new AuthResponseDto(200, "Login Successful", token);
            }
            catch (Exception ex)
            { 
                return new AuthResponseDto(500, $"Error While Login{ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,user.Role.ToString().ToLower())
            };
            var TokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
            };
            var token = tokenHandler.CreateToken(TokenDiscriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
