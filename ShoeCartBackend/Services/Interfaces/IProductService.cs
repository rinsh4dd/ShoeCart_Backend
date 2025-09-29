using ShoeCartBackend.Common;
using ShoeCartBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProductService
{
    // Create
    Task<ApiResponse<ProductDTO>> AddProductAsync(CreateProductDTO dto);

    // Read
    Task<ProductDTO?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();

    // Update
    Task<ApiResponse<ProductDTO>>UpdateProductAsync(UpdateProductDTO dto);

    // Delete
    Task<ApiResponse<string>> ToggleProductStatusAsync(int id);
}
