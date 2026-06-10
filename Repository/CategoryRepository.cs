using Microsoft.EntityFrameworkCore;
using EcommerceBlazor.Data;
using EcommerceBlazor.Repository.IRepository;

namespace EcommerceBlazor.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Category> CreateAsync(Category obj)
        {
            // 3. Create a temporary context instance
            using var context = await _contextFactory.CreateDbContextAsync();

            await context.Category.AddAsync(obj);
            await context.SaveChangesAsync();
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var category = await context.Category.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                context.Category.Remove(category);
                return (await context.SaveChangesAsync()) > 0;
            }

            return false;
        }

        public async Task<Category> GetAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var category = await context.Category.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                return category;
            }
            return new Category();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var categories = await context.Category.ToListAsync();
            return categories;
        }

        public async Task<Category> UpdateAsync(Category obj)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var category = await context.Category.FirstOrDefaultAsync(c => c.Id == obj.Id);
            if (category != null)
            {
                category.Name = obj.Name;
                await context.SaveChangesAsync();
                return category;
            }
            return obj;
        }
    }
}