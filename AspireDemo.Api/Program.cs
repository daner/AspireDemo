using AspireDemo.Api;
using AspireDemo.Api.Email;
using AspireDemo.Api.Messages;
using AspireDemo.Api.Messaging;
using AspireDemo.Api.Notifications;
using AspireDemo.Api.Weather;
using AspireDemo.Data;
using AspireDemo.ServiceDefaults;
using AspNetCore.SignalR.OpenTelemetry;
using Keycloak.AuthServices.Authentication;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ApplicationDbContext>("aspiredemo");

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSqlClientInstrumentation();
        tracing.AddSignalRInstrumentation();
        tracing.AddSource(MessageSender<EmailMessage>.ActivitySourceName);
    });

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policyBuilder => policyBuilder.RequireAuthenticatedUser() );
} );


builder.Services.AddTransient<IMessageSender<EmailMessage>>(sp =>
{
    return new MessageSender<EmailMessage>("email", sp.GetRequiredService<IConnection>(), sp.GetRequiredService<ILogger<MessageSender<EmailMessage>>>());
});

builder.Services.Configure<OpenWeatherMapOptions>(builder.Configuration.GetSection(nameof(OpenWeatherMapOptions)));

builder.Services.AddSignalR()
    .AddStackExchangeRedis(builder.Configuration.GetConnectionString("cache") ?? "")
    .AddHubInstrumentation();

builder.AddRabbitMQClient("rabbit");

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithKeycloak(builder.Configuration);

builder.AddRedisClient("cache");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AspireDemo v1");
        options.OAuthClientId(builder.Configuration["keycloak:resource"]);
        options.OAuthClientSecret(builder.Configuration["keycloak:credential:secret"]);
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/api/notifications")
    .RequireAuthorization();

app.MapMessageApi();
app.MapWeatherApi();
app.MapEmailApi();

app.Run();

await Log.CloseAndFlushAsync();
