namespace ShoeCartBackend.DTOs.CartDTO
{
    public class AddToCartDTO
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; } = 1;
    }

}
