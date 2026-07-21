using MediatR;
using PAS.Application.Commands.Funds.CreateFund;
using PAS.Application.Commands.Funds.PutFundNav;
using PAS.Application.Queries.Funds.GetFund;
using PAS.Application.Queries.Funds.GetFundList;

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

        return Results.CreatedAtRoute("GetFund", new { id }, new { id });
    }

    private static async Task<IResult> PutFundNav(Guid id, PutFundNavRequest request, ISender sender, CancellationToken ct) {
        await sender.Send(new PutFundNavCommand(id, request.Date, request.Value), ct);
        return Results.NoContent();
    }
}

public sealed record CreateFundRequest(string Name, string Isin, string Currency);
public sealed record PutFundNavRequest(DateOnly Date, decimal Value);