namespace PAS.Application.Dtos;

// A single NAV entry as exposed by the API — flattened from the Nav value object.
public sealed record NavDto(DateOnly Date, decimal Value);