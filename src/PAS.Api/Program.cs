using PAS.Asset.Api.Endpoints;
using PAS.Asset.Application;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseExceptionHandler();

app.MapOpenApi();           
app.MapScalarApiReference(); 

app.MapFundEndpoints();

app.Run();