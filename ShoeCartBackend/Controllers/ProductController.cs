using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeCartBackend.Common;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromForm] CreateProductDTO dto)
    {
        var product = await _productService.AddProductAsync(dto);
        return StatusCode(product.StatusCode, product);
    }

    [HttpPut]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update([FromForm] UpdateProductDTO dto)
    {
        var product = await _productService.UpdateProductAsync(dto);
        return StatusCode(product.StatusCode, product);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ToggleStatus([Range(1, int.MaxValue)] int id)
    {
        var newStatus = await _productService.ToggleProductStatusAsync(id);
        return StatusCode(newStatus.StatusCode, newStatus);
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        if (products == null || !products.Any())
            return NotFound(new ApiResponse<List<ProductDTO>>(404, "No products found in this category"));

        return Ok(new ApiResponse<List<ProductDTO>>(200, "Products fetched successfully", products.ToList()));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new ApiResponse<ProductDTO>(404, "Product not found"));

        return Ok(new ApiResponse<ProductDTO>(200, "Product fetched successfully", product));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(new ApiResponse<List<ProductDTO>>(200, "All products fetched successfully", products.ToList()));
    }
}
