using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using System.Threading.Tasks;

public interface IPaymentService
{
    Task<ApiResponse<object>> CreateOrderAsync(int userId, PaymentRequestDto dto);
    Task<ApiResponse<object>> VerifyPaymentAsync(PaymentVerifyDto dto);
}
