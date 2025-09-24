using ShoeCartBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetAllAsync();
    Task<Cart?> GetByIdAsync(int id);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(int id);
}
