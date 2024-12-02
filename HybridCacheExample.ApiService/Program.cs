using HybridCacheExample.ApiService;
using HybridCacheExample.ApiService.Data;
using HybridCacheExample.ApiService.Endpoints;
using HybridCacheExample.ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDb")));

builder.Services.AddStackExchangeRedisCache(
    config => config.Configuration = 
        builder.Configuration.GetConnectionString("Redis"));

#pragma warning disable EXTEXP0018

builder.Services.AddHybridCache(options =>
{
    options.MaximumKeyLength = 256;
    options.MaximumPayloadBytes = 5 * 1024 * 1024;
    options.ReportTagMetrics = false;
    
    options.DefaultEntryOptions = new HybridCacheEntryOptions()
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(10)
    };
});
   
// Serializer custom for type
// .AddSerializerFactory()<Article, CustomSerializer<Article>>();

// Serializer custom global
// .AddSerializerFactory<SerializerFactory>();

#pragma warning restore EXTEXP0018

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