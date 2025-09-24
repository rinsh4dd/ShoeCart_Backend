using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _repo;

    public ProductService(IGenericRepository<Product> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await _repo.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        await _repo.UpdateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
    }
}
