using AspireDemo.Api;
using AspireDemo.Api.Messages;
using AspireDemo.Api.Notifications;
using AspireDemo.Api.Weather;
using AspireDemo.Data;
using AspireDemo.ServiceDefaults;
using Keycloak.AuthServices.Authentication;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ApplicationDbContext>("aspiredemo");

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSqlClientInstrumentation();
    });

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policyBuilder => policyBuilder.RequireAuthenticatedUser() );
} );

builder.Services.Configure<OpenWeatherMapOptions>(builder.Configuration.GetSection(nameof(OpenWeatherMapOptions)));

builder.Services.AddSignalR().AddStackExchangeRedis(builder.Configuration.GetConnectionString("cache") ?? "");
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

app.Run();

await Log.CloseAndFlushAsync();
