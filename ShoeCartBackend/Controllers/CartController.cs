using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs.CartDTO;
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

    [Authorize(Policy = "Customer")]
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromForm] AddToCartDTO dto)
    {
        int userId = GetUserId();
        var response = await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Size, dto.Quantity);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = "user,admin")] 
    [HttpGet("{userId?}")]
    public async Task<IActionResult> GetCart(int? userId = null)
    {
        int currentUserId = GetUserId();

        // If the requester is a regular user, force their own userId
        if (!User.IsInRole("admin"))
        {
            userId = currentUserId;
        }
        else
        {
            // Admin must provide a userId to view a specific user's cart
            if (userId == null)
                return BadRequest(new { Status = 400, Message = "UserId is required for admin" });
        }

        var response = await _cartService.GetCartForUserAsync(userId.Value);
        return StatusCode(response.StatusCode, response);
    }


    [HttpPut("{cartItemId}")]
    [Authorize(Policy = "Customer")]

    public async Task<IActionResult> UpdateCartItem([FromForm]int cartItemId, [FromBody] int quantity)
    {
        int userId = GetUserId();
        var response = await _cartService.UpdateCartItemAsync(userId, cartItemId, quantity);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Policy = "Customer")]
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> RemoveCartItem(int cartItemId)
    {
        int userId = GetUserId();
        var response = await _cartService.RemoveCartItemAsync(userId, cartItemId);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Policy = "Customer")]
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        int userId = GetUserId();
        var response = await _cartService.ClearCartAsync(userId);
        return StatusCode(response.StatusCode, response);
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new UnauthorizedAccessException("User claim not found.");
        return int.Parse(claim.Value);
    }
}
