using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // ✅ POST: api/orders/checkout
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

        // ✅ GET: api/orders
        [HttpGet]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        // ✅ GET: api/orders/{orderId}
        [HttpGet("{orderId}")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = await _orderService.GetOrderByIdAsync(userId, orderId);
            return Ok(order);
        }

        // ✅ GET: api/orders/admin/all
        [HttpGet("admin/all")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // ✅ POST: api/orders/cancel/{orderId}
        [HttpPost("cancel/{orderId}")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            await _orderService.CancelOrderAsync(orderId);
            return Ok(new { message = "Order cancelled successfully." });
        }
    }
}
