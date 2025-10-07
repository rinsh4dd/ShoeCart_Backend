using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProductService
{
    Task<ApiResponse<ProductDTO>> AddProductAsync(CreateProductDTO dto);
    Task<ProductDTO?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<ApiResponse<ProductDTO>>UpdateProductAsync(UpdateProductDTO dto);
    Task<ApiResponse<string>> ToggleProductStatusAsync(int id);
Task<ApiResponse<IEnumerable<ProductDTO>>> GetFilteredProducts(
    string? name = null,
    int? categoryId = null,
    string? brand = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    bool? inStock = null,
    int page = 1,
    int pageSize = 20,
    string? sortBy = null,
    bool descending = false
);

}
