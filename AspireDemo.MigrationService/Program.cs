using AspireDemo.Data;
using AspireDemo.MigrationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<AppDbinitializer>();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource(AppDbinitializer.ActivitySourceName);
    });


builder.AddSqlServerDbContext<ApplicationDbContext>("aspiredemo");


var host = builder.Build();
host.Run();
