
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoeCartBackend.Data;
using ShoeCartBackend.DTOs.AuthDTO;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using ShoeCartBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IConfiguration _configuration;

        public AuthService( IGenericRepository<User> userRepo,
            IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            dto.Email = dto.Email.ToLower().Trim();
            dto.Name = dto.Name.Trim();
            dto.Password = dto.Password.Trim();

            var userExist = await _userRepo.GetAsync(u => u.Email == dto.Email);
            if (userExist != null)
                return new AuthResponseDto(409, "Email already exists");

            var newUser = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Roles.user
            };

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveChangesAsync();  

            return new AuthResponseDto(200, "Registration Successful");
        }


        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            dto.Email = dto.Email.Trim().ToLower();
            dto.Password = dto.Password.Trim();

            var user = await _userRepo.GetAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new AuthResponseDto(401, "Invalid username or password");

            if (user.IsBlocked)
                return new AuthResponseDto(403, "This Account has been Blocked!,Contact Admin");

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();  

            return new AuthResponseDto(200, "Login Successful", accessToken, refreshToken);
        }


        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new AuthResponseDto(401, "Invalid or expired refresh token");

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync(); 
            return new AuthResponseDto(200, "Token refreshed successfully", newAccessToken, newRefreshToken);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetAsync(u => u.RefreshToken == refreshToken);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync(); 

            return true;
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
                new Claim(ClaimTypes.Role, user.Role.ToString().ToLower())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            }; 

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
