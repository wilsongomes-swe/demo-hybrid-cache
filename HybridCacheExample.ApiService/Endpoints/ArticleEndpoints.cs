using HybridCacheExample.ApiService.Data;
using HybridCacheExample.ApiService.Models;
using Microsoft.EntityFrameworkCore;

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

    private static async Task<IResult> GetArticles(BlogDbContext db, int? categoryId = null)
    {
        var articlesQuery = db.Articles.AsNoTracking();
        if (categoryId is not null)
            articlesQuery = articlesQuery.Where(a => a.CategoryId == categoryId);

        var articles = await articlesQuery.ToListAsync();
        
        return Results.Ok(articles);
    }

    private static async Task<IResult> GetArticleById(Guid id, BlogDbContext db)
    {
        var article = await db.Articles.FindAsync(id);
        return article is not null ? Results.Ok(article) : Results.NotFound();
    }

    private static async Task<IResult> CreateArticle(ArticleInput input, BlogDbContext db)
    {
        var article = new Article
        {
            Title = input.Title,
            Slug = input.Slug,
            Content = input.Content,
            CategoryId = input.CategoryId
        };

        await db.Articles.AddAsync(article);
        await db.SaveChangesAsync();
        return Results.Created($"/articles/{article.Id}", article);
    }

    private static async Task<IResult> UpdateArticle(Guid id, ArticleInput input, BlogDbContext db)
    {
        var article = await db.Articles.FindAsync(id);
        if (article is null) return Results.NotFound();

        article.Title = input.Title;
        article.Content = input.Content;
        article.Slug = input.Slug;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteArticle(Guid id, BlogDbContext db)
    {
        var article = await db.Articles.FindAsync(id);
        if (article is null) return Results.NotFound();

        db.Articles.Remove(article);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    public record ArticleInput(string Title, int CategoryId, string Slug, string Content);
}
