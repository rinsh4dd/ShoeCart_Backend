public interface ICartService
{
    Task<Cart?> GetCartByUserIdAsync(int userId);           // Read
    Task AddToCartAsync(int userId, int productId, string size, int quantity); // Create
    Task UpdateCartItemAsync(int userId, int cartItemId, int quantity);       // Update
    Task RemoveCartItemAsync(int userId, int cartItemId);                     // Delete
    Task ClearCartAsync(int userId);                                         // Delete all
}
