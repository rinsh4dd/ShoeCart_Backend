using ShoeCartBackend.Enums;

namespace ShoeCartBackend.DTOs
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus NewStatus { get; set; }  // Directly use enum
    }
}
