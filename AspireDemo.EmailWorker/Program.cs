using AspireDemo.EmailWorker;
using AspireDemo.ServiceDefaults;
using Polly;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();

builder.AddSmtpClient("mailserver");

builder.Services.AddResiliencePipeline(EmailWorker.RetryPolicy, builder =>
{
    builder
        .AddRetry(new Polly.Retry.RetryStrategyOptions()
        {
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
        })
        .AddTimeout(TimeSpan.FromSeconds(10));
});

builder.Services.AddOptions<EmailOptions>()
    .BindConfiguration("Email")
    .ValidateDataAnnotations();

builder.Services.AddOpenTelemetry().WithTracing(tracing =>
{
    tracing.AddSource(EmailWorker.ActivitySourceName);
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
    await Task.Delay(20000);
}

host.Run();

await Log.CloseAndFlushAsync();
