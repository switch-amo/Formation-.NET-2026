using PAS.Api.Endpoints;
using PAS.Application.Repositories;
using PAS.Infrastructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add API services
builder.Services.AddOpenApi();

// Register MediatR
builder.Services.AddMediatR(configuration => {
    configuration.RegisterServicesFromAssembly(typeof(PAS.Application.Queries.GetFundList.GetFundListQuery).Assembly);
});

// Register application dependencies
builder.Services.AddScoped<IFundRepository, JsonFundRepository>();

var app = builder.Build();

// Configure HTTP request pipeline

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();

    app.MapScalarApiReference(options => {
        options
            .WithTitle("Policy Administration System API")
            .WithDefaultHttpClient(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient);
    });
}

// Map endpoints
app.MapFundEndpoints();

app.Run();