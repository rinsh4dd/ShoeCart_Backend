using ShoeCartBackend.Models;
using System.Threading.Tasks;

namespace ShoeCartBackend.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsByIdAsync(int orderId, int userId);
    }
}
