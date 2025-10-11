using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Common;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;

public class WishlistService : IWishlistService
{
    private readonly AppDbContext _context;

    public WishlistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<object>> GetWishlistAsync(int userId)
    {
        var items = await _context.Wishlists
            .Where(w => w.UserId == userId && !w.IsDeleted && w.Product.IsActive)
            .Include(w => w.Product)
            .ThenInclude(p=>p.Images)
            .ToListAsync();

        var result = items.Select(i => new
        {
            i.ProductId,
            i.Product.Name,
            i.Product.Price,
            i.Product.Brand,
            Images = i.Product.Images != null && i.Product.Images.Any()
         ? i.Product.Images.Select(img => $"data:{img.ImageMimeType};base64,{Convert.ToBase64String(img.ImageData)}")
         : null
        });
        return new ApiResponse<object>(200, "Wishlist fetched successfully", result);
    }
    public async Task<ApiResponse<string>> ToggleWishlistAsync(int userId, int productId)
    {
        var existing = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);
        if (existing != null)
        {
            _context.Wishlists.Remove(existing);
            await _context.SaveChangesAsync();
            return new ApiResponse<string>(200, "Product removed from wishlist");
        }
        var product = await _context.Products.FindAsync(productId);
        if (product == null || !product.IsActive)
            return new ApiResponse<string>(404, "Product not found or inactive");
        var wishlist = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };
        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
        return new ApiResponse<string>(200, "Product added to wishlist");
    }

}
