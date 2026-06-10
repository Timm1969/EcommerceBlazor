using Microsoft.AspNetCore.Identity;


namespace EcommerceBlazor.Data
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            // Resolve the DB context directly to safely work with the underlying role tracker
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roleNames = { "Admin", "Manager", "Customer" };

            foreach (var roleName in roleNames)
            {
                // Check the database table directly
                var roleExist = dbContext.Roles.Any(r => r.Name == roleName);
                if (!roleExist)
                {
                    // Add the role row directly to the DB context tracking loop
                    dbContext.Roles.Add(new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper() // Crucial for Identity comparisons
                    });
                }
            }

            // Save all changes directly to the SQL database tables
            await dbContext.SaveChangesAsync();
        }
    }
}
