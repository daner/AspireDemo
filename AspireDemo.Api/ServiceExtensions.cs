using Microsoft.OpenApi.Models;

namespace AspireDemo.Api;

public static class ServiceExtensions
{
    public static IServiceCollection AddSwaggerGenWithKeycloak(this IServiceCollection services, IConfiguration configuration)
    {
        var authServer = configuration["keycloak:auth-server-url"];
        var realm = configuration["keycloak:realm"];
        var url = $"{authServer}realms/{realm}/protocol/openid-connect/auth";
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "AspireDemo", Version = "v1" });
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(url),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" },
                            { "demo-api", "demo-api" }
                        }
                    }
                }
            });

            OpenApiSecurityScheme keycloakSecurityScheme = new()
            {
                Reference = new OpenApiReference
                {
                    Id = "Keycloak",
                    Type = ReferenceType.SecurityScheme,
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer",
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { keycloakSecurityScheme, Array.Empty<string>() },
            });
        });
        return services;
    }
}