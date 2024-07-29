using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IMatchPublisher
{
    Task NewMatchReady(string matchId, GameTypeEnum regime, List<string> usersIp);
    Task UserAddToMatch(string matchId, string userId, MatchStatusEnum matchStatus);
    Task MatchStatusUpdate(string matchId, MatchStatusEnum newStatus);
    Task MatchCancelled(string matchId, string reason);
}
