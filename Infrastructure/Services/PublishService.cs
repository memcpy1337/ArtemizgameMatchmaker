using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.MatchMakingEvents;
using Domain.Entities;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class PublishService : IPublishService
{
    private readonly IBus _bus;

    public PublishService(IBus bus)
    {
        _bus = bus;
    }

    public async Task NewMatchCreated(string matchId, GameTypeEnum regime)
    {
        await _bus.Publish<MatchNewEvent>(new MatchNewEvent() { MatchId = matchId, GameType = regime });
    }

    public async Task UserAddToMatch(string matchId, string userId, bool serverIsReady)
    {
        await _bus.Publish<MatchPlayerAddEvent>(new MatchPlayerAddEvent() 
        { 
            MatchId = matchId, 
            UserId = userId, 
            ServerWasReady = serverIsReady
        });
    }



}
