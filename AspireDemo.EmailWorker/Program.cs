using AspireDemo.EmailWorker;
using AspireDemo.ServiceDefaults;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();

builder.AddSmtpClient("mailserver");

builder.Services.AddOptions<EmailOptions>()
    .BindConfiguration("Email")
    .ValidateDataAnnotations();

builder.Services.AddOpenTelemetry().WithTracing(tracing =>
{
    tracing.AddSource(nameof(EmailWorker));
});

builder.AddRabbitMQClient("rabbit", configureConnectionFactory: config =>
{
    config.DispatchConsumersAsync = true;
});

builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();

if(builder.Environment.IsDevelopment())
{
    //Wait for containers to start
    await Task.Delay(15000);
}

host.Run();

await Log.CloseAndFlushAsync();
