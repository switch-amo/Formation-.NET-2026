using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using PAS.Api.Endpoints;
using PAS.Api.Handlers;
using PAS.Application;
using PAS.Infrastructure;
using Scalar.AspNetCore;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var vaultAddress = builder.Configuration["Vault:Address"];
if (!string.IsNullOrEmpty(vaultAddress)) {
    var vaultToken = builder.Configuration["Vault:Token"] ?? throw new InvalidOperationException("Vault:Token is required when Vault:Address is set.");

    var vaultClient = new VaultClient(new VaultClientSettings(vaultAddress, new TokenAuthMethodInfo(vaultToken)));
    var sqlSecret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "pas/sqlserver", mountPoint: "secret");

    var connectionString = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("PasAsset")) {
        Password = sqlSecret.Data.Data["password"].ToString()
    };
    builder.Configuration["ConnectionStrings:PasAsset"] = connectionString.ConnectionString;
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpClient("keycloak", client => client.BaseAddress = new Uri("https://localhost:8080"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = "https://localhost:8080/realms/PasAsset";
        options.Audience = "pas.api";
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