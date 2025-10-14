using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoeCartBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdClaim);

            var order = await _orderService.CreateOrderAsync(userId, dto);
            return Ok(order);
        }

        [HttpGet]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = await _orderService.GetOrderByIdAsync(userId, orderId);
            return Ok(order);
        }

        [HttpGet("admin/all")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }


        [HttpPost("admin/update-status/{orderId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, dto.NewStatus);
            return Ok(new ApiResponse<OrderDto>(StatusCodes.Status200OK, "Order status updated successfully", updatedOrder));
        }
        [HttpGet("admin/user/{userId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var response = await _orderService.GetOrdersByUserIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("cancel/{orderId}")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            await _orderService.CancelOrderAsync(orderId);
            return Ok(new { message = "Order cancelled successfully." });
        }

        [HttpGet("admin/dashboard")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetDashboardStats([FromQuery] string type = "all")
        {
            var response = await _orderService.GetDashboardStatsAsync(type);
            return StatusCode(response.StatusCode, response);
        }
    }
}
