var builder = DistributedApplication.CreateBuilder(args);

#region Redis

var cache = builder.AddRedis("cache")
                   .WithRedisCommander();

#endregion

#region SQL
var password = builder.AddParameter("sql-password", secret: true);
var sql = builder.AddSqlServer("sqlserver", password, 60123)
    .WithDataVolume("aspire-demo-sql-data");
#endregion

#region SEQ
var seq = builder.AddSeq("seq", 5341)
    .WithDataVolume("aspire-demo-seq-data");
#endregion

var api = builder.AddProject<Projects.AspireDemo_Api>("api")
    .WithReference(seq)
    .WithReference(cache)
    .WithReference(sql);

#region BFF
var bff = builder.AddProject<Projects.AspireDemo_Bff>("bff")
            .WithReference(api)
            .WithReference(seq);

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
