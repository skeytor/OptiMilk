using Microsoft.Extensions.Http.Resilience;
using MilkingYield.API.Clients;
using MilkingYield.API.Handlers;
using Polly;

namespace MilkingYield.API.Extentions;

internal static class PollyExtensions
{
    internal static IServiceCollection AddPollyPolicies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<TokenAuthorizationHandler>();

        services.AddHttpClient<CattleApiClient>(
            client => client.BaseAddress = new Uri(configuration["CattleApi:BaseUrl"]!))
            .AddHttpMessageHandler<TokenAuthorizationHandler>()
            .AddResilienceHandler("custom", pipeline =>
            {
                pipeline.AddTimeout(TimeSpan.FromSeconds(5));
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.9,
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(5)
                });
                pipeline.AddTimeout(TimeSpan.FromSeconds(1));
            });
        return services;
    }
}
