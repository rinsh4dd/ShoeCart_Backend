using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithDetailsAsync(int id);
}
