using HybridCacheExample.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace HybridCacheExample.ApiService.Data;

internal sealed class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options, IHostEnvironment env) : base(options)
    {
        if (env.IsDevelopment())
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>()
            .HasOne<Category>().WithMany()
            .HasForeignKey(a => a.CategoryId);

        modelBuilder.Seed();
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
}