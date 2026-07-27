using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAS.Application.Abstractions;
using PAS.Domain.Abstractions;

namespace PAS.Api.Handlers;

/// <summary>
/// Translates application/domain exceptions into RFC 7807 ProblemDetails responses
/// with the right HTTP status code, instead of a blanket 500.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler {
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService) => _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
        var problemDetails = MapToProblemDetails(exception);
        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails MapToProblemDetails(Exception exception) => exception switch {
        ValidationException validation => new HttpValidationProblemDetails(
            validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray())) {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred."
        },
        NotFoundException => new ProblemDetails {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found.",
            Detail = exception.Message
        },
        DomainException => new ProblemDetails {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "A business rule was violated.",
            Detail = exception.Message
        },
        _ => new ProblemDetails {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred."
        }
    };
}
