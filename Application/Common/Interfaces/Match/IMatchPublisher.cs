using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IMatchPublisher
{
    Task NewMatchCreated(string matchId, GameTypeEnum regime, List<string> usersIp);
    Task NewMatchReady(string matchId);
    Task UserAddToMatch(string matchId, string userId, MatchStatusEnum matchStatus);
    Task UserRemoveFromMatch(string userId, string matchId);
    Task MatchStart(string matchId);
    Task MatchCancelled(string matchId, MatchCancelEnum matchCancel);
    Task MatchEnd(string matchId, List<MatchPlayerResult> results);
}
