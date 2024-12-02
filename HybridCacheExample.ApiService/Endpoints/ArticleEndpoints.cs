using HybridCacheExample.ApiService.Data;
using HybridCacheExample.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace HybridCacheExample.ApiService.Endpoints;

public static class ArticleEndpoints
{
    public static RouteGroupBuilder MapArticleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetArticles).WithName("GetArticles");
        group.MapGet("{id}", GetArticleById).WithName("GetArticleById");
        group.MapPost("", CreateArticle).WithName("CreateArticle");
        group.MapPut("{id}", UpdateArticle).WithName("UpdateArticle");
        group.MapDelete("{id}", DeleteArticle).WithName("DeleteArticle");

        return group;
    }

    private static async Task<IResult> GetArticles(
        BlogDbContext db,
        CancellationToken cancellationToken,
        [FromQuery] int? categoryId = null)
    {
        var articlesQuery = db.Articles.AsNoTracking();
        if (categoryId is not null)
            articlesQuery = articlesQuery.Where(a => a.CategoryId == categoryId);

        var articles = await articlesQuery.ToListAsync(cancellationToken);
        
        return Results.Ok(articles);
    }

    private static async Task<IResult> GetArticleById(
        int id, 
        BlogDbContext db,
        HybridCache cache,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting article with id: {id}", id);

        var cacheEntryOptions = new HybridCacheEntryOptions()
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };
        
        var article = await cache.GetOrCreateAsync(
            $"article:{id}",
            (logger, id, db),
            async (state, ct) =>
            {
                state.logger.LogInformation($"Getting article {state.id} from DB");
                var articleFromDb = await state.db.Articles.FindAsync([state.id], ct);
                return articleFromDb;
            },
            cacheEntryOptions,
            tags: ["articles", "user:123", "category:456" ],
            cancellationToken: cancellationToken);
        
        
        return article is not null ? Results.Ok(article) : Results.NotFound();
    }

    private static async Task<IResult> CreateArticle(
        ArticleInput input, 
        BlogDbContext db, 
        CancellationToken cancellationToken)
    {
        var article = new Article
        {
            Title = input.Title,
            Slug = input.Slug,
            Content = input.Content,
            CategoryId = input.CategoryId
        };

        await db.Articles.AddAsync(article, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Created($"/articles/{article.Id}", article);
    }

    private static async Task<IResult> UpdateArticle(
        int id, 
        ArticleInput input, 
        BlogDbContext db,
        HybridCache cache,
        CancellationToken cancellationToken)
    {
        var article = await db.Articles.FindAsync([id], cancellationToken);
        if (article is null) return Results.NotFound();

        article.Title = input.Title;
        article.Content = input.Content;
        article.Slug = input.Slug;

        await db.SaveChangesAsync(cancellationToken);
        
        await cache.RemoveAsync($"article:{id}", cancellationToken);
        await cache.SetAsync($"article:{id}", article, 
            cancellationToken: cancellationToken);
        
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteArticle(
        int id, 
        BlogDbContext db,
        HybridCache cache,
        CancellationToken cancellationToken)
    {
        // await cache.RemoveAsync($"article:{id}", cancellationToken);
        await cache.RemoveByTagAsync("user:123");
        return Results.NoContent();
        
        var article = await db.Articles.FindAsync([id], cancellationToken);
        if (article is null) return Results.NotFound();

        db.Articles.Remove(article);
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }

    public record ArticleInput(string Title, int CategoryId, string Slug, string Content);
}
