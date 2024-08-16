using Application.Common.Interfaces;
using Contracts.Events.ServerEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class ServerGameEndConsumer : IConsumer<ServerGameEndEvent>
{
    private readonly IMatchService _matchService;
    public ServerGameEndConsumer(IMatchService matchService, IMatchRepository matchRepository)
    {
        _matchService = matchService;
    }

    public async Task Consume(ConsumeContext<ServerGameEndEvent> context)
    {
        await _matchService.MatchEnd(context.Message.ServerId, context.Message.WonSide); //NEED RENAME PROPERTY
    }
}
