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

        public bool InStock { get; set; } = true;  // flag if stock > 0
        public bool IsActive { get; set; } = true;

        public string SpecialOffer { get; set; }

        // Stock tracking
        public int CurrentStock { get; set; }  // new field

        // Sizes and images
        public ICollection<ProductSize> AvailableSizes { get; set; } = new List<ProductSize>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
