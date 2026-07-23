using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server

var sqlServer = builder.AddSqlServer("sqlserver")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume()
                .WithDbGate();

var database = sqlServer.AddDatabase("PasAsset");

// Keycloak

var kcUsername = builder.AddParameter("keycloak-username", "admin");
var kcPassword = builder.AddParameter("keycloak-password", secret: true);

var keycloak = builder.AddKeycloak("keycloak", 8080,
                    adminUsername: kcUsername,
                    adminPassword: kcPassword)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume()
                .WithRealmImport("./realms")
                .WithOtlpExporter();

// Projects

builder.AddProject<Projects.PAS_Api>("PAS-API")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(keycloak)
    .WaitFor(keycloak);

builder.Build().Run();