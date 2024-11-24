using HybridCacheExample.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace HybridCacheExample.ApiService.Data;

public static class BlogDbSeeder
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(GetCategories());
        modelBuilder.Entity<Article>().HasData(GetArticles());
    }

    private static IEnumerable<Category> GetCategories() =>
        [new Category { Id = 1, Name = "Technology" },
        new Category { Id = 2, Name = "Health" },
        new Category { Id = 3, Name = "Lifestyle" }];

    private static IEnumerable<Article> GetArticles() =>
        [new Article
        {
            Id = 1,
            Title = "Introduction to Hybrid Caching",
            Slug = "hybrid-caching-introduction",
            Content = "Hybrid caching combines the best of multiple caching strategies to improve performance.",
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new Article
        {
            Id = 2,
            Title = "10 Tips for a Healthier Life",
            Slug = "health-tips",
            Content = "Discover 10 tips to enhance your daily health routines.",
            CategoryId = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new Article
        {
            Id = 3,
            Title = "Maximizing Productivity in a Remote Work Environment",
            Slug = "remote-work-productivity",
            Content = "Learn how to stay productive while working remotely.",
            CategoryId = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }];
}