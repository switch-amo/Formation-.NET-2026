namespace PAS.Application.Abstractions;

public sealed class NotFoundException : Exception {
    public NotFoundException(string message) : base(message) { }
}