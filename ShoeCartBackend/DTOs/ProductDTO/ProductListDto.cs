// For listing products (lightweight, no heavy details)
public class ProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string Category { get; set; } = null!;
}
