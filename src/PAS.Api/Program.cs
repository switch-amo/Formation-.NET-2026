using Microsoft.EntityFrameworkCore;
using PAS.Asset.Api;
using PAS.Asset.Api.Endpoints;
using PAS.Asset.Application;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Each layer registers its own services — Program.cs stays tiny.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply any pending EF Core migrations at startup (creates the DB if needed).
using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseExceptionHandler();

app.MapOpenApi();            // /openapi/v1.json
app.MapScalarApiReference(); // Scalar UI at /scalar/v1

app.MapFundEndpoints();

app.Run();