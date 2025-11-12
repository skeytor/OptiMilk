using CattleManagement.API;
using CattleManagement.API.Data;
using CattleManagement.API.Extensions;
using CattleManagement.API.Models;
using CattleManagement.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlServer<AppDbContext>(
    builder.Configuration.GetConnectionString("CattleDatabase"),
    optionsAction: (options) => options.UseSeeding((context, _) =>
    {
        context.Set<Cattle>().AddRange(SampleData.Cattles);
        context.SaveChanges();
    }));

builder.Services.AddScoped<CattleService>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
