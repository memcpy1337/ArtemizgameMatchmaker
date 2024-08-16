using Application.Common.Interfaces;
using Contracts.Common.Models;
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

    public async Task MatchCancelled(string matchId, MatchCancelEnum matchCancel)
    {
        
        await _bus.Publish<MatchCancelEvent>(new MatchCancelEvent()
        {
            MatchId = matchId,
            Reason = matchCancel
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

    public async Task MatchStart(string matchId)
    {
        await _bus.Publish<MatchStartEvent>(new MatchStartEvent()
        {
            MatchId = matchId
        });
    }

    public async Task MatchEnd(string matchId, List<MatchPlayerResult> results)
    {
        await _bus.Publish<MatchEndEvent>(new MatchEndEvent()
        {
            MatchId = matchId,
            Results = results
        });
    }

    public async Task UserRemoveFromMatch(string userId, string matchId)
    {
        await _bus.Publish<MatchPlayerRemoveEvent>(new MatchPlayerRemoveEvent()
        {
            UserId = userId,
            MatchId = matchId
        });
    }
}
