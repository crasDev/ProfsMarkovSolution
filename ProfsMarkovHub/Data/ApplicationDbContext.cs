using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Models;
using ProfsMarkovHub.Models.Store;

namespace ProfsMarkovHub.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<ArticleTag> ArticleTags { get; set; } = null!;
    public DbSet<StoreItem> StoreItems { get; set; } = null!;
    public DbSet<StoreRedemption> StoreRedemptions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Article>()
            .HasIndex(a => a.Slug)
            .IsUnique();

        // Many-to-many relationship
        builder.Entity<ArticleTag>()
            .HasKey(at => new { at.ArticleId, at.TagId });

        builder.Entity<ArticleTag>()
            .HasOne(at => at.Article)
            .WithMany(a => a.ArticleTags)
            .HasForeignKey(at => at.ArticleId);

        builder.Entity<ArticleTag>()
            .HasOne(at => at.Tag)
            .WithMany(t => t.ArticleTags)
            .HasForeignKey(at => at.TagId);

        builder.Entity<StoreItem>()
            .HasIndex(s => s.ExternalId)
            .IsUnique();

        builder.Entity<StoreRedemption>()
            .HasIndex(r => r.ExternalId)
            .IsUnique();
    }
}
