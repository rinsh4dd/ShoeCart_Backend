using System.ComponentModel.DataAnnotations;

// "Upsert" means "Update or Insert"
public class UpsertProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public string Brand { get; set; }
    
}