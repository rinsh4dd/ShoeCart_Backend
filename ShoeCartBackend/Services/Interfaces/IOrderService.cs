using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderDto>> CreateOrderAsync(int userId, CreateOrderDto dto);

        Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(int userId);

        Task<OrderDto> GetOrderByIdAsync(int userId, int orderId);

        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        Task CancelOrderAsync(int orderId);

        Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        Task<ApiResponse<object>> GetDashboardStatsAsync(string type = "all");
    }
}
