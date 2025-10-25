using ShoeCartBackend.Models;

namespace ShoeCartBackend.Models
{
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }          // FK to Product
        public Product Product { get; set; }        // Navigation property

        public string ImageUrl { get; set; }        // Store Cloudinary URL instead of byte[]
        public bool IsMain { get; set; } = false;   // optional: main image shows first
    }
}
