using ShoeCartBackend.Repositories.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartWithItemsByUserIdAsync(int userId);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId, int userId);
    void Update(CartItem cartItem); 
}
