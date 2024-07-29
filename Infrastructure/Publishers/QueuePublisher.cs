using Application.Common.DTOs;
using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Publishers;

public class QueuePublisher : IQueuePublisher
{
    private readonly IBus _publishEndpoint;
    public QueuePublisher(IBus bus)
    {
        _publishEndpoint = bus;
    }

    public async Task AddedUserToQueueAsync(UserQueueRequest userQueueRequest)
    {
        await _publishEndpoint.Publish(new QueuePlayerAddEvent() { UserId = userQueueRequest.UserId, TicketId = userQueueRequest.Ticket.ToString() });
    }

    public async Task RemovedUserFromQueueAsync(string userId)
    {
        await _publishEndpoint.Publish(new QueuePlayerRemoveEvent() { UserId = userId });
    }
}
