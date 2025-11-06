using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using MilkingYield.API;
using MilkingYield.API.Clients;
using MilkingYield.API.Data;
using MilkingYield.API.Extentions;
using MilkingYield.API.Models;
using MilkingYield.API.Services;
using Polly;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("MilkingYieldDatabase"))
    .UseSeeding((context, _) =>
    {
        context.Set<MilkingRecord>().AddRange(SampleData.MilkingYields);
        context.SaveChanges();
    }));

builder.Services.AddHttpClient<CattleApiClient>(
    client => client.BaseAddress = new Uri(builder.Configuration["CattleApi:BaseUrl"]!))
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

builder.Services.AddScoped<MilkingSessionService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Servers = []);
    app.ApplyMigrations();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
