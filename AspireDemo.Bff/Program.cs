using AspireDemo.Bff;
using AspireDemo.ServiceDefaults;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddKeycloakWebApp(builder.Configuration.GetSection("Keycloak"), (options) =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.Headers["Location"] = context.RedirectUri;
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    }, options =>
    {
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.Scope.Add("offline_access");
    });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddHttpContextAccessor();

builder.AddSeqEndpoint("seq");
builder.Services.AddHttpForwarder();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToFile("index.html");

app.MapAuth();
app.MapBackendApi(builder.Configuration);

app.Run();