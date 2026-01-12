using Application.Common.Interfaces;
using Contracts.Events.ServerEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class ServerConnectionDataUpdateConsumer : IConsumer<ServerConnectionDataUpdateEvent>
{
    private readonly IMatchService _matchService;
    private readonly ILogger<ServerBadDownConsumer> _logger;
    public ServerConnectionDataUpdateConsumer(IMatchService matchService, ILogger<ServerBadDownConsumer> logger)
    {
        _matchService = matchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ServerConnectionDataUpdateEvent> context)
    {
        _logger.LogInformation($"Recieve bad down server {context.Message.ServerId}");
        await _matchService.ServerConnectionDataRecive(context.Message.MatchId, context.Message.Address, context.Message.Port);
    }
}
