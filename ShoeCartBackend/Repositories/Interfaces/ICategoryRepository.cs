using ShoeCartBackend.Models;

namespace ShoeCartBackend.Repositories.Interfaces
{
    public interface ICategoryRepository: IGenericRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);

    }
}
