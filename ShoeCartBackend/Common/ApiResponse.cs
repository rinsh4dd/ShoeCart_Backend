using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ShoeCartBackend.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }  
        public string? Message { get; set; } // optional message
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; } // the actual data

        // Constructors
        public ApiResponse() { }

        public ApiResponse(int statusCode, string? message = null, T data=default)
        {
            Data = data;
            Message = message;
            StatusCode =statusCode;
        }

        public ApiResponse(int statusCode,string message)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
