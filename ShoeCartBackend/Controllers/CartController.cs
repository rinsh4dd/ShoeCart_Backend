using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs.CartDTO;
using ShoeCartBackend.Models;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartDTO dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Size, dto.Quantity);

        return Ok(new
        {
            Status = 200,
            Message = "Product added to cart successfully"
        });
    }

   [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var cart = await _cartService.GetCartByUserIdAsync(userId);

        if (cart == null || !cart.Items.Any())
            return Ok(new ApiResponse<object>(200, "Cart is empty", new { Items = Array.Empty<object>() }));

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
                i.ImageData,
                i.ImageMimeType
            })
        };

        return Ok(new ApiResponse<object>(200, "Cart fetched successfully", cartResponse));
    }

    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] int quantity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await _cartService.UpdateCartItemAsync(userId, cartItemId, quantity);

        return Ok(new ApiResponse<string>(200, "Cart item quantity updated successfully"));
    }

    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> RemoveCartItem(int cartItemId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await _cartService.RemoveCartItemAsync(userId, cartItemId);

        return Ok(new ApiResponse<string>(200, "Cart item removed successfully"));
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await _cartService.ClearCartAsync(userId);

        return Ok(new ApiResponse<string>(200, "Cart cleared successfully"));
    }
}
