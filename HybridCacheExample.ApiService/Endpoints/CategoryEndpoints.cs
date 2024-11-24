using HybridCacheExample.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace HybridCacheExample.ApiService.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetCategories).WithName("GetCategories");
        group.MapGet("{id}", GetCategoryById).WithName("GetCategoryById");
        group.MapPost("", CreateCategory).WithName("CreateCategory");
        group.MapPut("{id}", UpdateCategory).WithName("UpdateCategory");
        group.MapDelete("{id}", DeleteCategory).WithName("DeleteCategory");

        return group;
    }

    private static async Task<IResult> GetCategories(BlogDbContext db)
    {
        var categories = await db.Categories.AsNoTracking().ToListAsync();
        return Results.Ok(categories);
    }

    private static async Task<IResult> GetCategoryById(int id, BlogDbContext db)
    {
        var category = await db.Categories.FindAsync(id);
        return category is not null ? Results.Ok(category) : Results.NotFound();
    }

    private static async Task<IResult> CreateCategory(CategoryInput input, BlogDbContext db)
    {
        var category = new Category { Name = input.Name };
        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();
        return Results.Created($"/categories/{category.Id}", category);
    }

    private static async Task<IResult> UpdateCategory(int id, CategoryInput input, BlogDbContext db)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return Results.NotFound();

        category.Name = input.Name;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteCategory(int id, BlogDbContext db)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return Results.NotFound();

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    public record CategoryInput(string Name);
}
