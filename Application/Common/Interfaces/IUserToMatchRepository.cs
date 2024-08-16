using Contracts.Common.Models.Enums;
using Domain.Entities;
using Netjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IUserToMatchRepository
{
    Task<UserToMatch?> AddUserToMatchAsync(User user, Match match, string ticket, string userIp, PlayerTypeEnum playerType, GameTypeEnum gameType);
    Task RemoveUserFromMatch(string userId);
    Task<bool> IsUserInMatch(User user);
    Task ClearAllUsersFromMatch(Match match);
    Task<List<UserToMatch>> GetNotConnectedUsersFromMatch(Match match);
    Task<UserToMatch?> GetUserPlayDataByTicket(string ticket, string matchId);
    Task SetUserConnectionStatus(string userId, bool isConnected);
    Task<Match?> GetMatchByParticipant(string userId);
    Task<List<UserToMatch>> GetActiveUsersInMatch(string matchId);
}
