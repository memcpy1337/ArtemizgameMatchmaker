using Application.Common.DTOs;
using Contracts.Common.Models.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IMatchService
{
    Task CreateMatch(Match match, List<UserQueueRequest> players);
    Task CancelMatch(string matchId, MatchCancelEnum matchCancel);
    Task MatchEnd(string matchId, PlayerTypeEnum wonSide);
    Task RemovePlayerFromMatch(string userId);
    Task UserConnectedToMatch(string userId);
    Task UserDisconnectedFromMatch(string userId);
    Task<bool> UpdateMatchStatus(MatchStatusEnum matchStatus, string matchId);
}
