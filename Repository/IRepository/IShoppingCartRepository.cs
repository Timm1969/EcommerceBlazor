using EcommerceBlazor.Data;

namespace EcommerceBlazor.Repository.IRepository
{
    public interface IShoppingCartRepository
    {
        public Task<bool> UpdateCartAsync(string userId, int product, int updateBy);
        public Task<IEnumerable<ShoppingCart>> GetAllAsync(string? userId);
        public Task<bool> ClearCartAsync(string? userId);

        public Task<int> GetTotalCartCartCountAsync(string? userId);
        public Task<bool> DeleteItemfromCartAsync(string userId, int productId);

    }
}
