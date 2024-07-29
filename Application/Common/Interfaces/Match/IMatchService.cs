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
    Task AddOrCreate(User user, string userIp, GameTypeEnum gameType, PlayerTypeEnum playerType);
    Task UpdateMatchStatus(MatchStatusEnum matchStatus, string matchId);
    Task CancelMatch(string matchId, string reasonMsg);
    Task RemovePlayerFromMatch(User user);
}
