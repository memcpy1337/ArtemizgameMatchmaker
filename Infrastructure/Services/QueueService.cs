using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public sealed class QueueService : IQueueService
{
    private readonly IPublishEndpoint _publishEndpoint;
    public QueueService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task AddedUserToQueueAsync(string userId, string ticketId, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new QueuePlayerAddEvent() { UserId = userId, TicketId = ticketId }, cancellationToken);
    }


    public async Task RemovedUserFromQueueAsync(string userId, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new QueuePlayerRemoveEvent() { UserId = userId }, cancellationToken);
    }
}
