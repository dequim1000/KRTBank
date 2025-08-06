using KRTBank.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Infrastructure.NotificationHandlers;

public class FraudPreventionHandler : INotificationHandler<AccountCreatedEvent>, INotificationHandler<AccountUpdatedEvent>, INotificationHandler<AccountDeletedEvent>
{
    private readonly ILogger<FraudPreventionHandler> _logger;

    public FraudPreventionHandler(ILogger<FraudPreventionHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fraud Prevention: Processing account created - ID: {notification.Id}");
        return Task.CompletedTask;
    }

    public Task Handle(AccountUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fraud Prevention: Processing account updated - ID: {notification.Id}");
        return Task.CompletedTask;
    }

    public Task Handle(AccountDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fraud Prevention: Processing account deleted - ID: {notification.Id}");
        return Task.CompletedTask;
    }
}
