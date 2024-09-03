using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace AspireDemo.Bff;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuth(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth");

        group.MapGet("me", (HttpContext context) =>
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var claims = context.User.Claims.Select(c => new { type = c.Type, value = c.Value }).ToArray();
                return Results.Json(new { isAuthenticated = true, claims});
            }

            return Results.Json(new { isAuthenticated = false, claims = new List<KeyValuePair<string, string>>() });
        });

        group.MapGet("login", ([FromQuery]string? redirectUrl) => Results.Challenge(new AuthenticationProperties
        {
            RedirectUri = redirectUrl ?? "/"
        }, [OpenIdConnectDefaults.AuthenticationScheme]));
        
        group.MapGet("logout", async (HttpContext context, [FromQuery]string? redirectUrl) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                {
                    RedirectUri = redirectUrl ?? "/"
                });
            })
            .RequireAuthorization();
        
        return group;
    }
}