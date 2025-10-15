using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Enums;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using ShoeCartBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<User> _userRepository;

        public OrderService(
            IMapper mapper,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<Order> orderRepository)
        {
            _mapper = mapper;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ApiResponse<OrderDto>>CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                return new ApiResponse<OrderDto>(400, "Cart is empty");

            decimal totalAmount = 0;
            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    return new ApiResponse<OrderDto>(404, $"Product {item.Name} not found");

                if (product.CurrentStock < item.Quantity)
                    return new ApiResponse<OrderDto>(400, $"Not enough stock for {product.Name}");

                totalAmount += product.Price * item.Quantity;
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
                PaymentStatus = dto.PaymentMethod == PaymentMethod.CashOnDelivery
                                    ? PaymentStatus.Pending
                                    : PaymentStatus.Completed,
                OrderStatus = dto.PaymentMethod == PaymentMethod.CashOnDelivery
                                    ? OrderStatus.Processing
                                    : OrderStatus.Pending,
                TotalAmount = totalAmount,
                CreatedOn = DateTime.UtcNow,
                Items = cart.Items.Select(c => new OrderItem
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

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            await _cartRepository.SaveChangesAsync();

            var orderDto = _mapper.Map<OrderDto>(order);
            return new ApiResponse<OrderDto>(200, "Order created successfully", orderDto);
        }

        // Get orders by user
        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new ApiResponse<IEnumerable<OrderDto>>(404, "User not found");

            var orders = await _orderRepository.GetAllAsync(
                predicate: o => o.UserId == userId,
                include: query => query
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
            );

            if (orders == null || !orders.Any())
                return new ApiResponse<IEnumerable<OrderDto>>(404, $"No orders found for user {user.Name}");

            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return new ApiResponse<IEnumerable<OrderDto>>(200, "User orders fetched successfully", orderDtos);
        }

        // Update order status
        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            order.OrderStatus = newStatus;
            if (order.PaymentMethod == PaymentMethod.CashOnDelivery && newStatus == OrderStatus.Delivered)
                order.PaymentStatus = PaymentStatus.Completed;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        // Get order by ID
        public async Task<OrderDto> GetOrderByIdAsync(int userId, int orderId)
        {
            var orders = await _orderRepository.GetAllAsync(
                predicate: o => o.Id == orderId && o.UserId == userId,
                include: query => query
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
            );

            var order = orders.FirstOrDefault();
            if (order == null)
                throw new Exception("Order not found.");

            return _mapper.Map<OrderDto>(order);
        }

        // Get all orders
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync(
                include: query => query
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .Include(o => o.User)
            );

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        // Cancel order
        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            order.OrderStatus = OrderStatus.Cancelled;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersByUserAsync(int userId)
        {
            // Fetch orders via generic repository
            var orders = await _orderRepository.GetAllAsync(
                predicate: o => o.UserId == userId,
                include: q => q
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
            );

            if (orders == null || !orders.Any())
                return new ApiResponse<IEnumerable<OrderDto>>(404, $"No orders found for user with ID {userId}");

            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return new ApiResponse<IEnumerable<OrderDto>>(200, "User orders fetched successfully", orderDtos);
        }

        // Dashboard stats
        public async Task<ApiResponse<object>> GetDashboardStatsAsync(string type = "all")
        {
            var orders = await _orderRepository.GetAllAsync(include: q => q.Include(o => o.Items));
            var deliveredOrders = orders.Where(o => o.OrderStatus == OrderStatus.Delivered).ToList();

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
