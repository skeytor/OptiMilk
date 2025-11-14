using MilkingYield.API.Extentions;
using MilkingYield.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabaseProvider(builder.Configuration);

// Add Polly policies for HttpClient resilience
builder.Services.AddPollyPolicies(builder.Configuration);

// Add Kafka producer and consumer services
builder.Services.AddKafkaConsumer(builder.Configuration);

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
