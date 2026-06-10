using Microsoft.EntityFrameworkCore;
using EcommerceBlazor.Data;
using EcommerceBlazor.Repository.IRepository;

namespace EcommerceBlazor.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public OrderRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<OrderHeader> CreateAsync(OrderHeader orderHeader)
        {
            orderHeader.OrderDate = DateTime.Now;
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.OrderHeader.AddAsync(orderHeader);
            await context.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<IEnumerable<OrderHeader>> GetAllAsync(string? userId = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (!string.IsNullOrEmpty(userId))
            {
                return await context.OrderHeader.Where(u => u.UserId == userId).ToListAsync();
            }
            return await context.OrderHeader.ToListAsync();
        }

        public async Task<OrderHeader> GetAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var order = await context.OrderHeader.Include(u => u.OrderDetails).FirstOrDefaultAsync(u => u.Id == id);
            return order ?? new OrderHeader(); // Returns an empty object if null
        }

        public async Task<OrderHeader> GetOrderBySessionIdAsync(string sessionId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var order = await context.OrderHeader.FirstOrDefaultAsync(u => u.SessionId == sessionId);
            return order ?? new OrderHeader(); // Returns an empty object if null
        }

        public async Task<OrderHeader> UpdateStatusAsync(int orderId, string status, string paymentIntentId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orderHeader = await context.OrderHeader.FirstOrDefaultAsync(u => u.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = status;
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderHeader.PaymentIntentId = paymentIntentId;
                }
                await context.SaveChangesAsync();
            }
            return orderHeader;
        }
    }
}
