using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
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

    // ================= Admin Endpoints =================
    [HttpPost("add")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductDTO dto)
    {
        var product = await _productService.AddProductAsync(dto);
        return Ok(new ApiResponse<ProductDTO>(200, "Product added successfully", product));
    }

    [HttpPut("update")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDTO dto)
    {
        var product = await _productService.UpdateProductAsync(dto);
        return Ok(new ApiResponse<ProductDTO>(200, "Product updated successfully", product));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(404, "Product not found"));
        return Ok(new ApiResponse<object>(200, "Product deleted successfully"));
    }

    // ================= Public Endpoints =================
    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        if (products == null || !products.Any())
            return NotFound(new ApiResponse<List<ProductDTO>>(404, "No products found in this category"));

        return Ok(new ApiResponse<List<ProductDTO>>(200, "Products fetched successfully", products.ToList()));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new ApiResponse<ProductDTO>(404, "Product not found"));

        return Ok(new ApiResponse<ProductDTO>(200, "Product fetched successfully", product));
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(new ApiResponse<List<ProductDTO>>(200, "All products fetched successfully", products.ToList()));
    }
}
