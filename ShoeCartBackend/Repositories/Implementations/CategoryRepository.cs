using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;

namespace ShoeCartBackend.Repositories.Implementations
{
    public class CategoryRepository:GenericRepository<Category>, ICategoryRepository
    {
        public readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
}
