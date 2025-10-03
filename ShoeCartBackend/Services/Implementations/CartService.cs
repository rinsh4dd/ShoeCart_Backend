﻿using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Common;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;
        
    public CartService(AppDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public async Task<ApiResponse<string>> AddToCartAsync(int userId, int productId, string size, int quantity)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(productId);
        if (product == null) return new ApiResponse<string>(404, "Product not found");
        if (!product.IsActive) return new ApiResponse<string>(400, "Product is deactivated");
        if (!product.InStock) return new ApiResponse<string>(400, "Product is out of stock");
        if (quantity < 1 || quantity > 5) return new ApiResponse<string>(400, "Quantity must be between 1 and 5");


        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted)
                    ?? new Cart { UserId = userId, Items = new List<CartItem>() };

        if (!_context.Carts.Local.Contains(cart)) _context.Carts.Add(cart);

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
        if (existingItem != null)
        {
            if (existingItem.Quantity + quantity > 5)
                return new ApiResponse<string>(400, "Quantity cannot exceed 5 per item");
            existingItem.Quantity += quantity;
        }
        else
        {
            var mainImage = product.Images.FirstOrDefault(i => i.IsMain);
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Size = size,
                Quantity = quantity,
                ImageData = mainImage?.ImageData,
                ImageMimeType = mainImage?.ImageMimeType
            });
        }

        await _context.SaveChangesAsync();
        return new ApiResponse<string>(200, "Product added to cart successfully");
    }

    public async Task<ApiResponse<object>> GetCartForUserAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

        if (cart == null || !cart.Items.Any())
            return new ApiResponse<object>(200, "Cart is empty", new { Items = Array.Empty<object>() });

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
                // 👇 Convert byte[] to Base64 string if exists
                Image = i.ImageData != null
                    ? $"data:{i.ImageMimeType};base64,{Convert.ToBase64String(i.ImageData)}"
                    : null
            })
        };

        return new ApiResponse<object>(200, "Cart fetched successfully", cartResponse);
    }


    public async Task<ApiResponse<string>> UpdateCartItemAsync(int userId, int cartItemId, int quantity)
    {
        if (quantity < 1 || quantity > 5)
            return new ApiResponse<string>(400, "Quantity must be between 1 and 5");

        var cartItem = await _context.CartItems.Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (cartItem == null) return new ApiResponse<string>(404, "Cart item not found");
        cartItem.Quantity = quantity;
        await _context.SaveChangesAsync();
        return new ApiResponse<string>(200, "Cart item quantity updated successfully");
    }

    public async Task<ApiResponse<string>> RemoveCartItemAsync(int userId, int cartItemId)
    {
        var cartItem = await _context.CartItems.Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (cartItem == null) return new ApiResponse<string>(404, "Cart item not found");

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return new ApiResponse<string>(200, "Cart item removed successfully");
    }

    public async Task<ApiResponse<string>> ClearCartAsync(int userId)
    {
        var cart = await _context.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

        if (cart != null) _context.CartItems.RemoveRange(cart.Items);

        await _context.SaveChangesAsync();
        return new ApiResponse<string>(200, "Cart cleared successfully");
    }
}
