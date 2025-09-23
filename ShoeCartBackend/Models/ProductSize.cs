using ShoeCartBackend.Models;

namespace ShoeCartBackend.Models
{
    public class ProductSize : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }   // Navigation property

        public string Size { get; set; }
    }
}
