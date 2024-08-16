using Application.Common.Interfaces;
using Contracts.Events.ServerEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class PlayerConnectionConsumer : IConsumer<ServerPlayerConnectedEvent>
{
    private readonly IMatchService _matchService;
    public PlayerConnectionConsumer(IMatchService matchService)
    {
        _matchService = matchService;
    }

    public async Task Consume(ConsumeContext<ServerPlayerConnectedEvent> context)
    {
       await _matchService.UserConnectedToMatch(context.Message.UserId);
    }
}
