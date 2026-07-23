using MediatR;
using PAS.Domain.Abstractions;

namespace PAS.Application.Abstractions;

/// <summary>
/// MediatR envelope for a domain event. Keeps the Domain layer free of any
/// MediatR dependency: the domain only knows <see cref="IDomainEvent"/>, and
/// the infrastructure wraps each event in this notification before publishing.
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification where TDomainEvent : IDomainEvent;
