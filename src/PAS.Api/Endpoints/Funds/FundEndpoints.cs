// Api/Endpoints/FundEndpoints.cs
using MediatR;
using PAS.Application.Queries.Funds;
using PAS.Asset.Application.Funds.Commands.CreateFund;
using PAS.Asset.Application.Funds.Commands.PutFundNav;

namespace PAS.Asset.Api.Endpoints;

public static class FundEndpoints {
    public static IEndpointRouteBuilder MapFundEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/funds").WithTags("Funds");

        group.MapGet("/", GetFundList).WithName("GetFundList");
        group.MapGet("/{id:guid}", GetFund).WithName("GetFund");
        group.MapPost("/", CreateFund).WithName("CreateFund");
        group.MapPut("/{id:guid}/nav", PutFundNav).WithName("PutFundNav");

        return app;
    }

    private static async Task<IResult> GetFundList(ISender sender, CancellationToken ct) {
        var result = await sender.Send(new GetFundListQuery(), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFund(Guid id, ISender sender, CancellationToken ct) {
        var result = await sender.Send(new GetFundQuery(id), ct);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> CreateFund(CreateFundRequest request, ISender sender, CancellationToken ct) {
        var id = await sender.Send(
            new CreateFundCommand(request.Name, request.Isin, request.Currency), ct);

        // 201 Created + Location header pointing to the GetFund route.
        return Results.CreatedAtRoute("GetFund", new { id }, new { id });
    }

    private static async Task<IResult> PutFundNav(Guid id, PutFundNavRequest request, ISender sender, CancellationToken ct) {
        await sender.Send(new PutFundNavCommand(id, request.Date, request.Value), ct);
        return Results.NoContent();
    }
}

// API contracts (request bodies) kept SEPARATE from the internal commands.
public sealed record CreateFundRequest(string Name, string Isin, string Currency);
public sealed record PutFundNavRequest(DateOnly Date, decimal Value);