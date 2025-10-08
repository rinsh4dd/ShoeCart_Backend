using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("test-error")]
    [AllowAnonymous] 

    public IActionResult TestError()
    {
        throw new Exception("Test exception from controller!");
    }

    [Authorize(Policy = "Customer")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddToCartDTO dto)
    {
        int userId = GetUserId();
        var response = await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Size, dto.Quantity);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("{userId?}")]
    public async Task<IActionResult> Get(int userId )
    {
        var response = await _cartService.GetCartForUserAsync(userId);
        return StatusCode(response.StatusCode, response);
    }
    [Authorize(Policy = "Customer")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        int UserId = GetUserId();
        var response = await _cartService.GetCartForUserAsync(UserId);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Policy = "Customer")]
    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateQuantityDTO dto)
    {
        int userId = GetUserId();
        var response = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto.Quantity);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Policy = "Customer")]
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> DeleteItem(int cartItemId)
    {
        int userId = GetUserId();
        var response = await _cartService.RemoveCartItemAsync(userId, cartItemId);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Policy = "Customer")]
    [HttpDelete]
    public async Task<IActionResult> Clear()
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
