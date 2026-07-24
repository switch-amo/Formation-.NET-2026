namespace PAS.Api.Endpoints;

public static class AuthEndpoints {
    private const string Realm = "PasAsset";
    private const string ClientId = "pas.api";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app) {
        // DEV-ONLY helper: exchanges test credentials for a Keycloak token via the
        // Resource Owner Password grant, so you can grab a bearer token straight from
        // Scalar. It must never be mapped in production.
        app.MapPost("/auth/token", GetToken)
            .WithTags("Auth")
            .WithName("GetToken")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> GetToken(IHttpClientFactory httpClientFactory, CancellationToken cancellationToken, TokenRequest? request = null) {
        request ??= new TokenRequest();
        var username = string.IsNullOrWhiteSpace(request.Username) ? "testuser" : request.Username;
        var password = string.IsNullOrWhiteSpace(request.Password) ? "Test123!" : request.Password;

        var form = new Dictionary<string, string> {
            ["grant_type"] = "password",
            ["client_id"] = ClientId,
            ["username"] = username,
            ["password"] = password,
            ["scope"] = "openid"
        };

        var client = httpClientFactory.CreateClient("keycloak");

        using var response = await client.PostAsync(
            $"realms/{Realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form),
            cancellationToken);

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return Results.Content(
            content: payload,
            contentType: "application/json",
            contentEncoding: null,
            statusCode: (int)response.StatusCode);
    }
}

public sealed record TokenRequest(string Username = "testuser", string Password = "Test123!");
