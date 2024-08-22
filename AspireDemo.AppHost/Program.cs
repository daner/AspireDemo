
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AspireDemo_Api>("api");

#region BFF
if (true) //(builder.ExecutionContext.IsPublishMode)
{
    builder
        .AddDockerfile("bffcontainer", "..", "AspireDemo.Bff/Dockerfile")
        .WithReference(api);
}

//var bff = builder.AddProject<Projects.AspireDemo_Bff>("bff")
//            .WithReference(api)
//            .ExcludeFromManifest();

//builder.AddNpmApp("web", "../AspireDemo.Web", "dev")
//    .WithReference(bff)
//    .WithHttpEndpoint(5173, env: "PORT", isProxied: false)
//    .ExcludeFromManifest();

#endregion

#region Standalone frontend
//builder.AddNpmApp("web", "../AspireDemo.Web", "dev")
//    .WithReference(api)
//    .WithHttpEndpoint(5173, env: "PORT", isProxied: false)
//    .PublishAsDockerFile();
#endregion

builder.Build().Run();
