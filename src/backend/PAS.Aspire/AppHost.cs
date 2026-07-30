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

var keycloak = builder.AddKeycloak("keycloak",
                    adminUsername: kcUsername,
                    adminPassword: kcPassword)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume()
                .WithRealmImport("./Realms")
                .WithOtlpExporter();

// HashiCorp Vault (mode dev, en mémoire) — aucun secret n'y est stocké pour l'instant

var vaultRootToken = builder.AddParameter("vault-root-token", "dev-root-token", secret: true);

var vault = builder.AddContainer("vault", "hashicorp/vault", "1.21")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithHttpEndpoint(port: 8200, targetPort: 8200, name: "http")
                .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", vaultRootToken)
                .WithEnvironment("VAULT_DEV_LISTEN_ADDRESS", "0.0.0.0:8200")
                .WithHttpHealthCheck("/v1/sys/health", endpointName: "http");

// Projects

var api = builder.AddProject<Projects.PAS_Api>("PAS-API")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(keycloak)
    .WaitFor(keycloak);

// Frontend (React + Vite)

builder.AddNpmApp("frontend", "../../frontend", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithEnvironment("VITE_API_TARGET", api.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

builder.Build().Run();