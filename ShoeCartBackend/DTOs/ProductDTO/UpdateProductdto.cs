using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class UpdateProductDTO
{
    [Required]
    public int Id { get; set; } // Product to update

    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Brand is required")]
    public string Brand { get; set; }
     public int CategoryId { get; set; }
    public string SpecialOffer { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int CurrentStock { get; set; }

    // Optional: activate/deactivate product
    public bool? IsActive { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one size is required.")]
    public List<string> AvailableSizes { get; set; } = new List<string>();

    // Optional: new images to add
    public List<IFormFile>? NewImages { get; set; }
}
