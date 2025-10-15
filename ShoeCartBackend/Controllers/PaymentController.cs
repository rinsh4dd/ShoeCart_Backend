using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using ShoeCartBackend.Common;
using ShoeCartBackend.Enums;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Order = ShoeCartBackend.Models.Order;

namespace ShoeCartBackend.Controllers
{
    
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] PaymentRequestDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized(new ApiResponse<object>(StatusCodes.Status401Unauthorized, "User not found"));

        var result = await _paymentService.CreateOrderAsync(userId, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerifyDto dto)
    {
        var result = await _paymentService.VerifyPaymentAsync(dto);
        return StatusCode(result.StatusCode, result);
    }
}
}
