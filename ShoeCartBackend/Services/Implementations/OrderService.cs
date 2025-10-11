using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Common;
using ShoeCartBackend.Data;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Enums;
using ShoeCartBackend.Models;
using ShoeCartBackend.Services.Interfaces;
using System;


namespace ShoeCartBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

         public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .Where(c => c.Cart.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            decimal totalAmount = 0;
            foreach (var item in cartItems)
            {
                if (item.Product.CurrentStock < item.Quantity)
                    throw new Exception($"Not enough stock for {item.Product.Name}");

                totalAmount += item.Product.Price * item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                BillingStreet = dto.BillingStreet,
                BillingCity = dto.BillingCity,
                BillingState = dto.BillingState,
                BillingZip = dto.BillingZip,
                BillingCountry = dto.BillingCountry,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentMethod == PaymentMethod.CashOnDelivery ? PaymentStatus.Pending : PaymentStatus.Completed,
                OrderStatus = dto.PaymentMethod == PaymentMethod.CashOnDelivery ? OrderStatus.Processing : OrderStatus.Pending,
                TotalAmount = totalAmount,
                Items = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Name = c.Name,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    Size = c.Size,
                    ImageData = c.ImageData,
                    ImageMimeType = c.ImageMimeType
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
        {

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            order.OrderStatus = newStatus;

            if (order.PaymentMethod == PaymentMethod.CashOnDelivery && newStatus == OrderStatus.Delivered)
            {
                order.PaymentStatus = PaymentStatus.Completed;
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }
        public async Task<OrderDto> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null)
                throw new Exception("Order not found");
                return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            order.OrderStatus = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
        }

        public async Task<ApiResponse<object>> GetDashboardStatsAsync(string type = "all")
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

            var deliveredOrders = orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .ToList();

            if (!deliveredOrders.Any())
                return new ApiResponse<object>(404, "No delivered orders found");

            var totalRevenue = deliveredOrders.Sum(o => o.TotalAmount);
            var totalProducts = deliveredOrders.Sum(o => o.Items.Sum(i => i.Quantity));
            var deliveredCount = deliveredOrders.Count;

            object data = type.ToLower() switch
            {
                "revenue" => new { TotalRevenue = totalRevenue },
                "products" => new { TotalProductsPurchased = totalProducts },
                "orders" => new { DeliveredOrdersCount = deliveredCount },
                _ => new
                {
                    TotalRevenue = totalRevenue,
                    TotalProductsPurchased = totalProducts,
                    DeliveredOrdersCount = deliveredCount
                }
            };

            return new ApiResponse<object>(200, "Dashboard stats fetched successfully", data);
        }

    }
}
