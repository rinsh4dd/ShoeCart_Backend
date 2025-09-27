using ShoeCartBackend.Common;

public interface ICartService
{
    // Read cart
    Task<ApiResponse<object>> GetCartForUserAsync(int userId);

    // Add product to cart
    Task<ApiResponse<string>> AddToCartAsync(int userId, int productId, string size, int quantity);

    // Update cart item quantity
    Task<ApiResponse<string>> UpdateCartItemAsync(int userId, int cartItemId, int quantity);

    // Remove single cart item
    Task<ApiResponse<string>> RemoveCartItemAsync(int userId, int cartItemId);

    // Clear all items in cart
    Task<ApiResponse<string>> ClearCartAsync(int userId);
}
