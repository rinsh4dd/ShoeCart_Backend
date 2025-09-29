using ShoeCartBackend.Common;

public interface IWishlistService
{
    Task<ApiResponse<object>> GetWishlistAsync(int userId);
    Task<ApiResponse<string>> ToggleWishlistAsync(int userId, int productId);
}
