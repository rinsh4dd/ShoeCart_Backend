using System.Collections.Generic;

public class Cart:BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
