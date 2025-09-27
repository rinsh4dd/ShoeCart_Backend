using System.Collections.Generic;

namespace ShoeCartBackend.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string Brand { get; set; }

        // Inventory & status
        public bool InStock { get; set; } = true;
        public bool IsActive { get; set; } = true;  // ✅ new field

        public string SpecialOffer { get; set; }

        // Navigation properties
        public ICollection<ProductSize> AvailableSizes { get; set; } = new List<ProductSize>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
