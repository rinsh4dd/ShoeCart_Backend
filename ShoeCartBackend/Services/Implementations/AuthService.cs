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
        public AuthService(AppDbContext appDbContext,IGenericRepository<User>userRepo)
        {
            _appDbContext = appDbContext;
            _userRepo =userRepo;
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
                    Role = Roles.user
                };
                await _userRepo.AddAsync(newUser);
                return new AuthResponseDto(200, "Registration Successfull");

            }
            catch (Exception ex)
            {
                return new AuthResponseDto(500, $"Error Adding User{ex.Message}");
            }
        }

        //public async Task<User?> GetUserByEmailAsync(string email)
        //{
        //    var users = await _userRepo.GetAllAsync();
        //    return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        //}

        //public async Task<string> LoginAsync(string email, string password)
        //{
        //    var users = await _userRepo.GetAllAsync();
        //    var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        //    if (user == null || !VerifyPassword(password, user.PasswordHash))
        //        throw new Exception("Invalid email or password.");

        //    if (user.IsBlocked)
        //        throw new Exception("User is blocked. Contact admin.");

        //    return GenerateJwt(user);
        //}

        //private string HashPassword(string password)
        //{
        //    return BCrypt.Net.BCrypt.HashPassword(password);
        //}

        //private bool VerifyPassword(string password, string hash)
        //{
        //    return BCrypt.Net.BCrypt.Verify(password, hash);
        //}

        //private string GenerateJwt(User user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_jwtSecret);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //            new Claim(ClaimTypes.Name, user.Name),
        //            new Claim(ClaimTypes.Email, user.Email),
        //            new Claim(ClaimTypes.Role, user.Role.ToString())
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(key),
        //            SecurityAlgorithms.HmacSha256Signature
        //        )
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
