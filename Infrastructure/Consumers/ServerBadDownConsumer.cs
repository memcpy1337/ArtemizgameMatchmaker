using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.ServerEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class ServerBadDownConsumer : IConsumer<ServerBadDownEvent>
{
    private readonly IMatchService _matchService;
    private readonly ILogger<ServerBadDownConsumer> _logger;
    public ServerBadDownConsumer(IMatchService matchService, ILogger<ServerBadDownConsumer> logger)
    {
        _matchService = matchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ServerBadDownEvent> context)
    {
        _logger.LogInformation($"Recieve bad down server {context.Message.ServerId}");
       await _matchService.CancelMatch(context.Message.MatchId, MatchCancelEnum.InternalError);
    }
}
