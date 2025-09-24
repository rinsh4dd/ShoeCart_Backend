namespace ShoeCartBackend.DTOs.AuthDTO
{
    public class AuthResponseDto
    {
        public int StatusCode { get; set; }  
        public string Message { get; set; }=null!;
        public string? Token { get; set; } 
       

        public AuthResponseDto(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public AuthResponseDto(int statusCode,string message,string token)
        {
            StatusCode = statusCode;
            Message = message;
            Token = token;
        }
    }
}
 