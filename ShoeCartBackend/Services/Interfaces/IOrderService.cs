using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
        Task<OrderDto> GetOrderByIdAsync(int userId, int orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task CancelOrderAsync(int orderId);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<ApiResponse<object>> GetDashboardStatsAsync(string type = "all");

        // 🆕 Admin endpoint to fetch orders for any specific user
        Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(int userId);
    }
}
