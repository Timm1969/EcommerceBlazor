using Microsoft.EntityFrameworkCore;
using EcommerceBlazor.Data;
using EcommerceBlazor.Repository.IRepository;

namespace EcommerceBlazor.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IWebHostEnvironment webHostEnvironment)
        {
            _contextFactory = contextFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<Product> CreateAsync(Product obj)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Product.AddAsync(obj);
            await context.SaveChangesAsync();
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Product.FirstOrDefaultAsync(u => u.Id == id);

            if (product == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/', '\\'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            if (product != null)
            {
                context.Product.Remove(product);
                return (await context.SaveChangesAsync()) > 0;
            }
            return false;
        }

        public async Task<Product> GetAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Product.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                return product;
            }
            return new Product();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var products = await context.Product.Include(u => u.Category).ToListAsync();
            return products;
        }

        public async Task<Product> UpdateAsync(Product obj)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Product.FirstOrDefaultAsync(p => p.Id == obj.Id);
            if (product != null)
            {
                product.Name = obj.Name;
                product.Price = obj.Price;
                product.Description = obj.Description;
                product.CategoryId = obj.CategoryId;
                product.ImageUrl = obj.ImageUrl;
                context.Product.Update(product);
                await context.SaveChangesAsync();
                return product;
            }
            return obj;
        }
    }
}
