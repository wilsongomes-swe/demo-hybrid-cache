using HybridCacheExample.ApiService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite("Data Source=blog.db"));

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.AddServiceDefaults();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/articles", async (BlogDbContext db) =>
        await db.Articles.AsNoTracking().ToListAsync())
    .WithName("GetArticles");

app.MapGet("/articles/{id}", async (Guid id, BlogDbContext db) =>
        await db.Articles.FindAsync(id) is { } article
            ? Results.Ok(article)
            : Results.NotFound())
    .WithName("GetArticleById");

app.MapPost("/articles", async (ArticleInput input, BlogDbContext db) =>
{
    var article = new Article {
        Title = input.Title,
        Slug = input.Slug,
        Content = input.Content 
    };
    await db.Articles.AddAsync(article);
    await db.SaveChangesAsync();
    return Results.Created($"/articles/{article.Id}", article);
}).WithName("CreateArticle");

app.MapPut("/articles/{id}", async (Guid id, ArticleInput input, BlogDbContext db) =>
{
    var article = await db.Articles.FindAsync(id);

    if (article is null)
        return Results.NotFound();

    article.Title = input.Title;
    article.Content = input.Content;
    article.Slug = input.Slug;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("UpdateArticle");

app.MapDelete("/articles/{id}", async (Guid id, BlogDbContext db) =>
{
    var article = await db.Articles.FindAsync(id);

    if (article is null)
        return Results.NotFound();

    db.Articles.Remove(article);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("DeleteArticle");

app.Run();

public record ArticleInput(string Title, string Slug, string Content);