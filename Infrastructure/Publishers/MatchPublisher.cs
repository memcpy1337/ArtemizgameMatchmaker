using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.MatchMakingEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Publishers;

public class MatchPublisher : IMatchPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<MatchPublisher> _logger;

    public MatchPublisher(IBus bus, ILogger<MatchPublisher> logger)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task MatchCancelled(string matchId, string reason)
    {
        await _bus.Publish<MatchCancelEvent>(new MatchCancelEvent()
        {
            MatchId = matchId,
            ReasonMsg = reason
        });
    }

    public async Task MatchStatusUpdate(string matchId, MatchStatusEnum newStatus)
    {
        await _bus.Publish<MatchStatusUpdateEvent>(new MatchStatusUpdateEvent() 
        { 
            MatchId = matchId, 
            NewStatus = newStatus 
        });
    }

    public async Task NewMatchReady(string matchId, GameTypeEnum regime, List<string> usersIp)
    {
        await _bus.Publish<MatchNewEvent>(new MatchNewEvent()
        {
            MatchId = matchId,
            GameType = regime,
            UsersIp = usersIp
        });
    }

    public async Task UserAddToMatch(string matchId, string userId, MatchStatusEnum matchStatus)
    {
        await _bus.Publish<MatchPlayerAddEvent>(new MatchPlayerAddEvent()
        {
            MatchId = matchId,
            UserId = userId,
            MatchStatus = matchStatus
        });
    }
}
