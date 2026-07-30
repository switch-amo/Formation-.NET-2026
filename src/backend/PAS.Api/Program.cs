using Microsoft.AspNetCore.Authorization;
using PAS.Api.Endpoints;
using PAS.Api.Handlers;
using PAS.Application;
using PAS.Infrastructure.Persistence;
using PAS.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpClient("keycloak", client => client.BaseAddress = new Uri("https://localhost:8080"));

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "PasAsset",
        options => {
            options.Audience = "pas.api";
            // Development only — disable HTTPS metadata validation.
            if (builder.Environment.IsDevelopment()) {
                options.RequireHttpsMetadata = false;
            }
        });

builder.Services.AddAuthorization(options => {
    // Every endpoint requires an authenticated user unless it has [AllowAnonymous].
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

app.Services.ApplyMigrations();

app.UseAuthentication();
app.UseAuthorization();  
app.UseExceptionHandler();

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();
app.MapFundEndpoints();

if (app.Environment.IsDevelopment()) {
    app.MapAuthEndpoints();
}

app.Run();