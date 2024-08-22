var builder = DistributedApplication.CreateBuilder(args);

#region Redis

var cache = builder.AddRedis("cache")
                   .WithRedisCommander();

#endregion

#region SQL
var password = builder.AddParameter("sql-password", secret: true);
var sql = builder.AddSqlServer("sqlserver", password, 60123);

#endregion

var api = builder.AddProject<Projects.AspireDemo_Api>("api")
    .WithReference(cache)
    .WithReference(sql);

#region BFF
if (builder.ExecutionContext.IsPublishMode)
{
    builder
        .AddDockerfile("bffcontainer", "..", "AspireDemo.Bff/Dockerfile")
        .WithHttpEndpoint(targetPort: 8080)
        .WithReference(api);
}

var bff = builder.AddProject<Projects.AspireDemo_Bff>("bff")
            .WithReference(api)
            .ExcludeFromManifest();

builder.AddNpmApp("web", "../AspireDemo.Web", "dev")
    .WithReference(bff)
    .WithHttpEndpoint(5173, env: "PORT", isProxied: false)
    .ExcludeFromManifest();

#endregion

#region Standalone frontend
//builder.AddNpmApp("web", "../AspireDemo.Web", "dev")
//    .WithReference(api)
//    .WithHttpEndpoint(5173, env: "PORT", isProxied: false)
//    .PublishAsDockerFile();
#endregion

builder.Build().Run();
