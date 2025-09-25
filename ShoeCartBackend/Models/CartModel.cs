using System.Collections.Generic;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation property
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
