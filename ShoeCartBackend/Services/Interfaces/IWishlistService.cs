public interface IWishlistService
{
    Task<IEnumerable<Wishlist?>> GetAllAsync();
    Task<Wishlist?> GetByIdAsync(int id);
    Task AddAsync(Wishlist wishlist);
    Task UpdateAsync(Wishlist wishlist);
    Task DeleteAsync(int id);
}