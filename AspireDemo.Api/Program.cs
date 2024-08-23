using AspireDemo.Api.Weather;
using AspireDemo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ApplicationDbContext>("aspiredemo");

builder.Services.Configure<OpenWeatherMapOptions>(builder.Configuration.GetSection(nameof(OpenWeatherMapOptions)));

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddSeqEndpoint("seq");
builder.AddRedisClient("cache");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapWeather();

app.Run();
