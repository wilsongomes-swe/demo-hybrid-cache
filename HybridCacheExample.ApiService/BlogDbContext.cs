using Microsoft.EntityFrameworkCore;

namespace HybridCacheExample.ApiService;

internal sealed class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options, IHostEnvironment env) : base(options)
    {
        if(env.IsDevelopment())
            Database.EnsureCreated();
    }

    public DbSet<Article> Articles { get; set; }
}