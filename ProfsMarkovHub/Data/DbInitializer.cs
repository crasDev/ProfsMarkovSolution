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

            // Seed Sample Articles
            if (!context.Articles.Any())
            {
                var aiToolsArticle = new Article
                {
                    Title = "5 AI Tools Every Developer Should Use in 2026",
                    Slug = "5-ai-tools-developers-2026",
                    Content = @"## Introduction

The AI tool landscape is overwhelming. Every week, a new \"revolutionary\" coding assistant launches. I've tested over 20 AI tools while building SyncTaskPro, my SaaS project management tool.

Only 5 made it to my daily workflow.

Here's why they survived—and how they save me 10+ hours per week.\n

## 1. GitHub Copilot - Your AI Pair Programmer
...
**Conclusion**

AI tools won't replace developers in 2026. They'll replace developers who don't use AI tools.

The developers thriving in 2026 will be those who:
- Leverage AI for repetitive tasks
- Focus on architecture and problem-solving
- Ship faster without sacrificing quality\n",
                    PublishedAt = DateTime.UtcNow,
                    AuthorId = adminUser?.Id,
                    ImageUrl = "https://picsum.photos/id/237/800/400"
                };

                context.Articles.Add(aiToolsArticle);

                await context.SaveChangesAsync();
            }
        }
    }
}
