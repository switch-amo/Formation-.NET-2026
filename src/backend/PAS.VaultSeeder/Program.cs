using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

string RequireValue(string key) => configuration[key] ?? throw new InvalidOperationException($"Configuration '{key}' is required.");

var vaultClient = new VaultClient(new VaultClientSettings(RequireValue("Vault:Address"), new TokenAuthMethodInfo(RequireValue("Vault:Token"))));

await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
    path: "pas/sqlserver",
    data: new Dictionary<string, object> {
        ["username"] = "sa",
        ["password"] = RequireValue("Sql:Password")
    },
    mountPoint: "secret");

await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
    path: "pas/keycloak",
    data: new Dictionary<string, object> {
        ["username"] = RequireValue("Keycloak:Username"),
        ["password"] = RequireValue("Keycloak:Password")
    },
    mountPoint: "secret");