using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CattleManagement.API.Extensions;

internal static class AuthExtensions
{
    internal static IServiceCollection AddKeycloak(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://auth-server:8080/realms/microservices";
                options.RequireHttpsMetadata = false; // Required because you're using HTTP
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:7003/realms/microservices",
                    ValidateAudience = true,
                    ValidAudience = "account"
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
