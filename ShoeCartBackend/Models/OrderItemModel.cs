using Microsoft.EntityFrameworkCore;

namespace ShoeCartBackend.Models
{
    public class OrderItem : BaseEntity
    {
        // Relationships
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Snapshot fields (kept for history even if product changes later)
        public string Name { get; set; }

        [Precision(18, 2)]  // Ensures proper money format in SQL
        public decimal Price { get; set; }

        public string Size { get; set; }
        public int Quantity { get; set; }

        public string ImageUrl { get; set; }

    }
}
