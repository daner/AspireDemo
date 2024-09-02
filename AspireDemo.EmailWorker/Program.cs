using AspireDemo.EmailWorker;
using AspireDemo.ServiceDefaults;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();
builder.AddRabbitMQClient("rabbit");
builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();

if(builder.Environment.IsDevelopment())
{
    //Wait for containers to start
    await Task.Delay(20000);
}

host.Run();

await Log.CloseAndFlushAsync();
