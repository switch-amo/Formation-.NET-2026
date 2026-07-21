var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
                .WithLifetime(ContainerLifetime.Persistent);

var database = sqlServer.AddDatabase("PasAsset");

builder.AddProject<Projects.PAS_Api>("PAS-API")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();