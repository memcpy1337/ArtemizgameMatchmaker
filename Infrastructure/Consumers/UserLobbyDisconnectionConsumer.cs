using Application.Common.Interfaces;
using Contracts.Events.UserEvents;
using Forbids;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class UserLobbyDisconnectionConsumer : IConsumer<UserExitQueuePlayEvent>
{
    private readonly IQueueService _queueService;

    public UserLobbyDisconnectionConsumer(IQueueService queueService)
    {
        _queueService = queueService;
    }

    public async Task Consume(ConsumeContext<UserExitQueuePlayEvent> context)
    {
        await _queueService.RemoveUserFromQueueAsync(context.Message.UserId, CancellationToken.None);
    }
}
