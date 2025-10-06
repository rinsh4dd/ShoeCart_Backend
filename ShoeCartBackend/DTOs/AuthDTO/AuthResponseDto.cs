using System.Text.Json.Serialization;

namespace ShoeCartBackend.DTOs.AuthDTO
{
    public class AuthResponseDto
    {
        public int StatusCode { get; set; }  
        public string Message { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get; set; }

        // Default constructor for just status & message
        public AuthResponseDto(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        // Constructor for Access Token only (old behavior)
        public AuthResponseDto(int statusCode, string message, string accessToken)
        {
            StatusCode = statusCode;
            Message = message;
            AccessToken = accessToken;
        }

        // Constructor for Access + Refresh Token
        public AuthResponseDto(int statusCode, string message, string accessToken, string refreshToken)
        {
            StatusCode = statusCode;
            Message = message;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
