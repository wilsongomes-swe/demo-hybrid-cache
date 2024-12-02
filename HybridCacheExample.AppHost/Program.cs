var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("SqlServer")
    .WithLifetime(ContainerLifetime.Persistent);
var db = sqlServer.AddDatabase("BlogDb", "blog");

var redis = builder.AddRedis("Redis")
    .WithRedisInsight();

builder.AddProject<Projects.HybridCacheExample_ApiService>("ApiService")
    .WithReference(db)
    .WithReference(redis)
    .WithReplicas(2)
    .WithExternalHttpEndpoints();

builder.Build().Run();
