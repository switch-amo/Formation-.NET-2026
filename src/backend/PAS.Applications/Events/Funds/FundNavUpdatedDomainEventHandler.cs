using MediatR;
using Microsoft.Extensions.Logging;
using PAS.Application.Abstractions;
using PAS.Domain.Funds.Events;

namespace PAS.Application.Events.Funds;

/// <summary>
/// Example domain-event handler. It is discovered and registered automatically
/// by MediatR (RegisterServicesFromAssembly) and invoked by the
/// DomainEventsInterceptor after the aggregate is persisted.
/// </summary>
public sealed class FundNavUpdatedDomainEventHandler : INotificationHandler<DomainEventNotification<FundNavUpdatedDomainEvent>> {
    private readonly ILogger<FundNavUpdatedDomainEventHandler> _logger;

    public FundNavUpdatedDomainEventHandler(ILogger<FundNavUpdatedDomainEventHandler> logger)
        => _logger = logger;

    public Task Handle(DomainEventNotification<FundNavUpdatedDomainEvent> notification, CancellationToken cancellationToken) {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "NAV updated for fund {FundId} ({Isin}): {Value} on {Date:yyyy-MM-dd}",
            domainEvent.FundId, domainEvent.Isin, domainEvent.Value, domainEvent.Date);

        return Task.CompletedTask;
    }
}
