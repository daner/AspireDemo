var builder = DistributedApplication.CreateBuilder(args);

#region Auth
var keycloakAdmin = builder.AddParameter("keycloak-admin");
var keycloakPassword = builder.AddParameter("keycloak-password", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8080, adminUsername: keycloakAdmin, adminPassword: keycloakPassword)
    .WithDataVolume("keycloak-data");
#endregion

#region Messaging
var rabbitAdmin = builder.AddParameter("rabbit-admin");
var rabbitPassword = builder.AddParameter("rabbit-password", secret: true);
var rabbit = builder.AddRabbitMQ("rabbit", rabbitAdmin, rabbitPassword)
    .WithDataVolume()
    .WithManagementPlugin();
#endregion

#region Redis

var cache = builder.AddRedis("cache")
    .WithRedisCommander();

#endregion

#region SEQ

var seq = builder.AddSeq("seq", 5341)
    .WithDataVolume();

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

#region API
var api = builder.AddProject<Projects.AspireDemo_Api>("api")
    .WithReference(seq)
    .WithReference(cache)
    .WithReference(sql)
    .WithReference(rabbit);
#endregion

#region Frontend and BFF

var bff = builder.AddProject<Projects.AspireDemo_Bff>("bff")
    .WithReference(api)
    .WithReference(seq);

//For production builds this project is handled by the Dockerfile in the BFF project
builder.AddNpmApp("web", "../AspireDemo.Web", "dev")
    .WithReference(bff)
    .WithHttpEndpoint(5173, env: "PORT", isProxied: false)
    .ExcludeFromManifest();

#endregion

#region Worker
builder.AddProject<Projects.AspireDemo_EmailWorker>("aspiredemo-emailworker")
    .WithReference(seq)
    .WithReference(rabbit);
#endregion



builder.Build().Run();