using HybridCacheExample.ApiService;
using HybridCacheExample.ApiService.Endpoints;
using HybridCacheExample.ApiService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDb")));

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.AddServiceDefaults();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGroup("/articles")
    .WithTags("Articles")
    .MapArticleEndpoints();

app.MapGroup("/categories")
    .WithTags("Categories")
    .MapCategoryEndpoints();

app.Run();
