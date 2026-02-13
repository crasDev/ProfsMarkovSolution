using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Models;

namespace ProfsMarkovHub.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure DB is created and migrated
            await context.Database.MigrateAsync();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Seed Admin User
            var adminEmail = "admin@profsmarkov.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "P@ssword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Sample Article
            if (!context.Articles.Any())
            {
                var article = new Article
                {
                    Title = "Welcome to Profs Markov Hub",
                    Slug = "welcome-profs-markov-hub",
                    Content = "This is the first post on the new blog engine. Stay tuned for more content!",
                    PublishedAt = DateTime.UtcNow,
                    AuthorId = adminUser?.Id,
                    ImageUrl = "https://picsum.photos/seed/markov/800/400"
                };
                context.Articles.Add(article);
                await context.SaveChangesAsync();
            }
        }
    }
}
