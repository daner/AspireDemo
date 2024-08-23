var builder = DistributedApplication.CreateBuilder(args);

#region Redis

var cache = builder.AddRedis("cache")
    .WithRedisCommander();

#endregion

#region SEQ

var seq = builder.AddSeq("seq", 5341)
    .WithDataVolume("aspire-demo-seq-data");

#endregion

#region SQL

var password = builder.AddParameter("sql-password", secret: true);
var sql = builder.AddSqlServer("sqlserver", password, 60123)
    .WithDataVolume("aspire-demo-sql-data")
    .AddDatabase("aspiredemo");

builder.AddProject<Projects.AspireDemo_MigrationService>("migrationservice")
    .WithReference(sql)
    .WithReference(seq);

#endregion

var api = builder.AddProject<Projects.AspireDemo_Api>("api")
    .WithReference(seq)
    .WithReference(cache)
    .WithReference(sql);

#region Frontend and BFF

var bff = builder.AddProject<Projects.AspireDemo_Bff>("bff")
    .WithReference(api)
    .WithReference(seq);

builder.AddNpmApp("web", "../AspireDemo.Web", "dev")
    .WithReference(bff)
    .WithReference(api)
    .WithHttpEndpoint(5173, env: "PORT", isProxied: false)
    .PublishAsDockerFile();

#endregion

builder.Build().Run();