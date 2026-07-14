using MediatR;
using PAS.Application.Queries.GetFundList;

namespace PAS.Api.Endpoints;

public static class FundEndpoints {
    public static IEndpointRouteBuilder MapFundEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/funds");

        group.MapGet("/", GetFundList);

        return app;
    }

    private static async Task<IResult> GetFundList(ISender sender, CancellationToken cancellationToken) {
        var result = await sender.Send(new GetFundListQuery(), cancellationToken);

        return Results.Ok(result);
    }
}