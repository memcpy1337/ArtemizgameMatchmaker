﻿using Application.Common.Interfaces;
using Contracts.Events.ServerEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class PlayerDisconnectionConsumer : IConsumer<ServerPlayerDisconnectedEvent>
{
    private readonly IMatchService _matchService;
    public PlayerDisconnectionConsumer(IMatchService matchService)
    {
        _matchService = matchService;
    }

    public async Task Consume(ConsumeContext<ServerPlayerDisconnectedEvent> context)
    {
        await _matchService.UserDisconnectedFromMatch(context.Message.UserId);
    }
}