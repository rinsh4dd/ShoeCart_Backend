using ShoeCartBackend.Common;
using ShoeCartBackend.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepo;
    private readonly IProductRepository _productRepo; 

    public WishlistService(IWishlistRepository wishlistRepo, IProductRepository productRepo)
    {
        _wishlistRepo = wishlistRepo;
        _productRepo = productRepo;
    }

    public async Task<ApiResponse<object>> GetWishlistAsync(int userId)
    {
        var items = await _wishlistRepo.GetWishlistByUserAsync(userId);

        var result = items.Select(i => new
        {
            i.ProductId,
            i.Product.Name,
            i.Product.Price,
            i.Product.Brand,
            Images = i.Product.Images != null && i.Product.Images.Any()
            ? i.Product.Images.Select(img => img.ImageUrl)
            : null
        });

        return new ApiResponse<object>(200, "Wishlist fetched successfully", result);
    }

    public async Task<ApiResponse<string>> ToggleWishlistAsync(int userId, int productId)
    {
        var existing = await _wishlistRepo.GetWishlistItemAsync(userId, productId);

        if (existing != null)
        {
            await _wishlistRepo.RemoveWishlistItemAsync(existing);
            return new ApiResponse<string>(200, "Product removed from wishlist");
        }

        var product = await _productRepo.GetByIdAsync(productId); // Or fetch via repo
        if (product == null || !product.IsActive)
            return new ApiResponse<string>(404, "Product not found or inactive");

        var wishlist = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        await _wishlistRepo.AddWishlistItemAsync(wishlist);

        return new ApiResponse<string>(200, "Product added to wishlist");
    }
}
