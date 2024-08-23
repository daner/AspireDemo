using AspireDemo.Data;
using AspireDemo.MigrationService;
using AspireDemo.ServiceDefaults;
using OpenTelemetry.Trace;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();

builder.Services.AddHostedService<AppDbInitializer>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource(AppDbInitializer.ActivitySourceName);
        tracing.AddSqlClientInstrumentation();
    });

builder.AddSqlServerDbContext<ApplicationDbContext>("aspiredemo");

var host = builder.Build();
host.Run();

await Log.CloseAndFlushAsync();