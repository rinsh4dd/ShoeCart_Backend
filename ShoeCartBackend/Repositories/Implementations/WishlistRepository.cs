using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class WishlistRepository : IWishlistRepository
{
    private readonly AppDbContext _context;

    public WishlistRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Wishlist>> GetWishlistByUserAsync(int userId)
    {
        return await _context.Wishlists
            .Where(w => w.UserId == userId && !w.IsDeleted && w.Product.IsActive)
            .Include(w => w.Product)
            .ThenInclude(p => p.Images)
            .ToListAsync();
    }

    public async Task<Wishlist> GetWishlistItemAsync(int userId, int productId)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);
    }

    public async Task AddWishlistItemAsync(Wishlist wishlist)
    {
        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveWishlistItemAsync(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
        await _context.SaveChangesAsync();
    }
}
