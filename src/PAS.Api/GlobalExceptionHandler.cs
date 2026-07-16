// Api/GlobalExceptionHandler.cs
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAS.Asset.Application.Abstractions;
using PAS.Domain.Abstractions;

namespace PAS.Asset.Api;

// Turns domain/application exceptions into proper HTTP responses,
// so handlers and endpoints never deal with status codes themselves.
public sealed class GlobalExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
        var statusCode = exception switch {
            DomainException => StatusCodes.Status400BadRequest,   // broken invariant
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails {
            Status = statusCode,
            Title = exception.GetType().Name,
            Detail = exception.Message
        }, cancellationToken);

        return true; // exception handled
    }
}