using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using ShoeCartBackend.Common;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;

public class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    public CartService( IProductRepository productRepository,ICartRepository cartRepository)
    {
        _productRepository = productRepository;
        _cartRepository = cartRepository;
    }
    public async Task<ApiResponse<string>> AddToCartAsync(int userId, int productId, string size, int quantity)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(productId);
        if (product == null)
            return new ApiResponse<string>(404, "Product not found");
        if (!product.IsActive)
            return new ApiResponse<string>(400, "Product is deactivated");
        if (!product.InStock)
            return new ApiResponse<string>(400, "Product is out of stock");
        if (quantity < 1 || quantity > 5)
            return new ApiResponse<string>(400, "Quantity must be between 1 and 5");

        // Get user's cart or create new
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            await _cartRepository.AddAsync(cart); // New cart → Add
        }

        // Check for existing item
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
        if (existingItem != null)
        {
            if (existingItem.Quantity + quantity > 5)
                return new ApiResponse<string>(400, "Quantity cannot exceed 5 per item");

            existingItem.Quantity += quantity;
            // **No Update call here if EF is tracking the entity**
        }
        else
        {
            var firstImage = product.Images?.FirstOrDefault();
            var cartItem = new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Size = size,
                Quantity = quantity,
                ImageData = firstImage?.ImageData,
                ImageMimeType = firstImage?.ImageMimeType
            };
            cart.Items.Add(cartItem);
            // EF will track new CartItem automatically
        }

        // Save all changes at once
        await _cartRepository.SaveChangesAsync();

        return new ApiResponse<string>(200, "Product added to cart successfully");
    }



    public async Task<ApiResponse<object>> GetCartForUserAsync(int userId)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);

        if (cart == null || cart.Items == null || !cart.Items.Any())
        {
            return new ApiResponse<object>(200, "Cart is empty", new { Items = Array.Empty<object>() });
        }

        var cartResponse = new
        {
            TotalQuantity = cart.Items.Sum(i => i.Quantity),
            TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity),
            Items = cart.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.Name,
                i.Price,
                i.Size,
                i.Quantity,
                Image = i.ImageData != null
                    ? $"data:{i.ImageMimeType};base64,{Convert.ToBase64String(i.ImageData)}"
                    : null,
                i.ImageMimeType
            })
        };

        return new ApiResponse<object>(200, "Cart fetched successfully", cartResponse);
    }



    public async Task<ApiResponse<string>> UpdateCartItemAsync(int userId, int cartItemId, int quantity)
    {
        if (quantity < 1 || quantity > 5)
            return new ApiResponse<string>(400, "Quantity must be between 1 and 5");

        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId, userId);
        if (cartItem == null)
            return new ApiResponse<string>(404, "Cart item not found");

        cartItem.Quantity = quantity;
        _cartRepository.Update(cartItem);

        await _cartRepository.SaveChangesAsync();

        return new ApiResponse<string>(200, "Cart item quantity updated successfully");
    }


    public async Task<ApiResponse<string>> RemoveCartItemAsync(int userId, int cartItemId)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
        if (cart == null || !cart.Items.Any())
            return new ApiResponse<string>(404, "Cart or items not found");

        var cartItem = cart.Items.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
            return new ApiResponse<string>(404, "Cart item not found");

        cart.Items.Remove(cartItem);

        await _cartRepository.SaveChangesAsync();

        return new ApiResponse<string>(200, "Cart item removed successfully");
    }



    public async Task<ApiResponse<string>> ClearCartAsync(int userId)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);

        if (cart == null || cart.Items == null || !cart.Items.Any())
            return new ApiResponse<string>(200, "Cart is already empty");

        foreach (var item in cart.Items)
        {
            await _cartRepository.DeleteAsync(item.Id);
        }

        await _cartRepository.SaveChangesAsync();

        return new ApiResponse<string>(200, "Cart cleared successfully");
    }

}
