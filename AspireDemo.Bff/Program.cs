using AspireDemo.ServiceDefaults;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSeqEndpoint("seq");
builder.Services.AddHttpForwarder();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseStaticFiles();

var group = app.MapGroup("/api");

group.MapForwarder("{*path}", builder.Configuration["services:api:http:0"] ?? "");

app.MapFallbackToFile("index.html");

app.Run();
