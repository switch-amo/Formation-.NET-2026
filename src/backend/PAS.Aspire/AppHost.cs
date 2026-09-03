using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server

var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sqlServer = builder.AddSqlServer("sqlserver", sqlPassword)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume()
                .WithDbGate();

var database = sqlServer.AddDatabase("PasAsset");

// Keycloak

var kcUsername = builder.AddParameter("keycloak-username", "admin");
var kcPassword = builder.AddParameter("keycloak-password", secret: true);

var keycloak = builder.AddKeycloak("keycloak", port: 8080,
                    adminUsername: kcUsername,
                    adminPassword: kcPassword)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume()
                .WithRealmImport("./Realms")
                .WithOtlpExporter();

// HashiCorp Vault (mode dev, en mémoire)

var vaultRootToken = builder.AddParameter("vault-root-token", "dev-root-token", secret: true);

var vault = builder.AddContainer("vault", "hashicorp/vault", "1.21")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithHttpEndpoint(port: 8200, targetPort: 8200, name: "http")
                .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", vaultRootToken)
                .WithEnvironment("VAULT_DEV_LISTEN_ADDRESS", "0.0.0.0:8200");

// Alternative
//
//vault.OnResourceReady(async (_, _, cancellationToken) => {
//    var vaultClient = new VaultClient(new VaultClientSettings(
//        vault.GetEndpoint("http").Url,
//        new TokenAuthMethodInfo(await vaultRootToken.Resource.GetValueAsync(cancellationToken))));

//    await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
//        path: "pas/sqlserver",
//        data: new Dictionary<string, object> {
//            ["username"] = "sa",
//            ["password"] = (await sqlPassword.Resource.GetValueAsync(cancellationToken))!
//        },
//        mountPoint: "secret");

//    await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
//        path: "pas/keycloak",
//        data: new Dictionary<string, object> {
//            ["username"] = (await kcUsername.Resource.GetValueAsync(cancellationToken))!,
//            ["password"] = (await kcPassword.Resource.GetValueAsync(cancellationToken))!
//        },
//        mountPoint: "secret");
//});

var vaultSeeder = builder.AddProject<Projects.PAS_VaultSeeder>("vault-seeder")
    .WithEnvironment("Vault__Address", vault.GetEndpoint("http"))
    .WithEnvironment("Vault__Token", vaultRootToken)
    .WithEnvironment("Sql__Password", sqlPassword)
    .WithEnvironment("Keycloak__Username", kcUsername)
    .WithEnvironment("Keycloak__Password", kcPassword)
    .WaitFor(vault);

// Projects

var api = builder.AddProject<Projects.PAS_Api>("PAS-API")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithEnvironment("Vault__Address", vault.GetEndpoint("http"))
    .WithEnvironment("Vault__Token", vaultRootToken)
    .WaitForCompletion(vaultSeeder);

// Frontend (React + Vite)

builder.AddNpmApp("frontend", "../../frontend", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithEnvironment("VITE_API_TARGET", api.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

builder.Build().Run();