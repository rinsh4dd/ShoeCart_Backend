using ShoeCartBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
    Task<ProductDTO?> GetProductByIdAsync(int id);

}
