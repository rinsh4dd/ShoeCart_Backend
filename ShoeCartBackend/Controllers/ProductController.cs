using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Models;
using ShoeCartBackend.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products/category/4
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        if (products == null || !products.Any())
            return NotFound(new ApiResponse<List<ProductDTO>>(404,"No products found in this category"));
        return Ok(new ApiResponse<List<ProductDTO>>(200, "Products fetched successfully", products.ToList()));
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new ApiResponse<ProductDTO> (404, "Product Not Found"));

        return Ok(new ApiResponse<ProductDTO>(200, "Product fetched successfully",product));
    }
}
