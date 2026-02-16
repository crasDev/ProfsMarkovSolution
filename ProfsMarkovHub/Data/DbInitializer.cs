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

            // Seed Sample Articles based on current Level Up / content pipeline
            if (!context.Articles.Any())
            {
                var now = DateTime.UtcNow;

                var articles = new List<Article>
                {
                    new Article
                    {
                        Title = "Diablo Warlock Triple Reveal + D2R Steam Launch",
                        Slug = "diablo-warlock-triple-reveal-d2r-steam-launch",
                        Content = @"# Diablo Warlock Triple Reveal + D2R Steam Launch

This is a test seed article mirroring the Level Up content pipeline. In the real workflow the full script lives in content-drafts/2026-02-16_Diablo_Warlock_Triple_Reveal_D2R_Steam_Launch_blog.md.

Use this post to validate routing, markdown rendering and card layouts.",
                        PublishedAt = now.AddDays(-1),
                        AuthorId = adminUser?.Id,
                        ImageUrl = "https://picsum.photos/id/1015/800/400"
                    },
                    new Article
                    {
                        Title = "Sony State of Play 2026 – Winners & Losers",
                        Slug = "sony-state-of-play-2026-winners-and-losers",
                        Content = @"# Sony State of Play 2026 – Winners & Losers

Seed article for testing blog listing, detail pages and tag ribbons. Represents the real State of Play breakdown used in social + blog drafts.",
                        PublishedAt = now.AddHours(-12),
                        AuthorId = adminUser?.Id,
                        ImageUrl = "https://picsum.photos/id/1025/800/400"
                    },
                    new Article
                    {
                        Title = "Steam Reviews Now Show PC Specs – Why It Matters",
                        Slug = "steam-reviews-now-show-pc-specs",
                        Content = @"# Steam Reviews Now Show PC Specs – Why It Matters

This article is a short test version of the real blog draft that explains why seeing hardware next to reviews changes how you read performance complaints.",
                        PublishedAt = now,
                        AuthorId = adminUser?.Id,
                        ImageUrl = "https://picsum.photos/id/1040/800/400"
                    },
                    new Article
                    {
                        Title = "Steam Machine vs RAM Crisis",
                        Slug = "steam-machine-vs-ram-crisis",
                        Content = @"# Steam Machine vs RAM Crisis

Seed post representing the discussion about launchers, overlays and how much RAM gets burned before you even launch a game.",
                        PublishedAt = now.AddHours(-6),
                        AuthorId = adminUser?.Id,
                        ImageUrl = "https://picsum.photos/id/1043/800/400"
                    },
                    new Article
                    {
                        Title = "120 AI Tools That Redefine How Developers Work",
                        Slug = "120-ai-tools-that-redefine-how-developers-work",
                        Content = @"# 120 AI Tools That Redefine How Developers Work

Seed version of the long-form article. Use it to confirm pagination, reading experience and typography for bigger guides.",
                        PublishedAt = now.AddDays(-2),
                        AuthorId = adminUser?.Id,
                        ImageUrl = "https://picsum.photos/id/1062/800/400"
                    }
                };

                context.Articles.AddRange(articles);

                await context.SaveChangesAsync();
            }
        }
    }
}
