using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ShoeCartBackend.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderWithItemsByIdAsync(int orderId, int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }
    }
}
