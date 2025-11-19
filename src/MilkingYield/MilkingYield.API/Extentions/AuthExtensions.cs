using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MilkingYield.API.Extentions;

internal static class AuthExtensions
{
    internal static IServiceCollection AddKeycloak(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var keycloakOptions = configuration.GetSection("Keycloak").Get<KeycloakOptions>();

                options.Authority = keycloakOptions!.Authority;
                options.RequireHttpsMetadata = false; // Required because you're using HTTP

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = keycloakOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = keycloakOptions.Audience,
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine($"[JWT] Auth failed: {ctx.Exception.GetType().Name} - {ctx.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = ctx =>
                    {
                        if (!ctx.Handled)
                            Console.WriteLine($"[JWT] Challenge: error={ctx.Error} desc={ctx.ErrorDescription}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        Console.WriteLine("[JWT] Token validated.");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
