using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ShoeCartBackend.Repositories.Implementations
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }


        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId, int userId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId
                                           && ci.Cart.UserId == userId
                                           && !ci.Cart.IsDeleted);
        }

        public void Update(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
        }

        public async Task ClearCartForUserAsync(int userId)
        {
            var cart = await GetCartWithItemsByUserIdAsync(userId);
            if (cart != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
        }

    }
}
