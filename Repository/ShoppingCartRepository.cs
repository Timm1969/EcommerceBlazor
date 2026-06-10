using Microsoft.EntityFrameworkCore;
using EcommerceBlazor.Data;
using EcommerceBlazor.Repository.IRepository;

namespace EcommerceBlazor.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        public ShoppingCartRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> ClearCartAsync(string? userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var cartItems = await context.ShoppingCart.Where(u => u.UserId == userId).ToListAsync();
            context.ShoppingCart.RemoveRange(cartItems);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync(string? userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.ShoppingCart.Where(u => u.UserId == userId).Include(u => u.Product).ToListAsync();
        }

		public async Task<int> GetTotalCartCartCountAsync(string? userId)
		{
            using var context = await _contextFactory.CreateDbContextAsync();

            // Asks SQL server to directly add up the 'Count' column for this user
            return await context.ShoppingCart
                .Where(u => u.UserId == userId)
                .SumAsync(item => item.Count);
        }

		public async Task<bool> UpdateCartAsync(string userId, int productId, int updateBy)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            var cart = await context.ShoppingCart.FirstOrDefaultAsync(u => u.UserId == userId && u.ProductId == productId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    ProductId = productId,
                    Count = updateBy
                };

                await context.ShoppingCart.AddAsync(cart);
            }
            else
            {
                cart.Count += updateBy;
                if (cart.Count <= 0)
                {
                    context.ShoppingCart.Remove(cart);
                }
            }
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteItemfromCartAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            var cart = await context.ShoppingCart.FirstOrDefaultAsync(u => u.UserId == userId && u.ProductId == productId);

            if (cart == null)
            {
                return false;
            }

            context.ShoppingCart.Remove(cart);
            return await context.SaveChangesAsync() > 0;

        }

    }
}
