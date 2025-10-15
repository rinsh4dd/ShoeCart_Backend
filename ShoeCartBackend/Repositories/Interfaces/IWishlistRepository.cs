using ShoeCartBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IWishlistRepository
{
    Task<List<Wishlist>> GetWishlistByUserAsync(int userId);
    Task<Wishlist> GetWishlistItemAsync(int userId, int productId);
    Task AddWishlistItemAsync(Wishlist wishlist);
    Task RemoveWishlistItemAsync(Wishlist wishlist);
}
