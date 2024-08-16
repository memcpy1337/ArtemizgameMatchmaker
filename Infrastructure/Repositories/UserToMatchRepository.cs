using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Match = Domain.Entities.Match;

namespace Infrastructure.Repositories;

public class UserToMatchRepository : IUserToMatchRepository
{
    private readonly IApplicationDbContext _applicationDb;

    public UserToMatchRepository(IApplicationDbContext applicationDb)
    {
        _applicationDb = applicationDb;
    }

    public async Task<UserToMatch?> AddUserToMatchAsync(User user, Match match, string ticket, string userIp, PlayerTypeEnum playerType, GameTypeEnum gameType)
    {
        var newUserToMatch = new UserToMatch()
        {
            MatchId = match.Id,
            UserId = user.Id,
            UserIp = userIp,
            UserType = playerType,
            IsActive = true,
            Ticket = ticket
        };

        await _applicationDb.UserToMatches.AddAsync(newUserToMatch);

        await _applicationDb.SaveChangesAsync(CancellationToken.None);

        return newUserToMatch;
    }

    public async Task ClearAllUsersFromMatch(Match match)
    {
        await _applicationDb.UserToMatches
            .Where(x => x.Match == match && x.IsActive)
            .ExecuteUpdateAsync(b => b.SetProperty(x => x.IsActive, false));

        await _applicationDb.SaveChangesAsync(CancellationToken.None);
    }

    public async Task SetUserConnectionStatus(string userId, bool isConnected)
    {
        await _applicationDb.UserToMatches
            .Include(x => x.User)
            .Where(x => x.User.Id == userId && x.IsActive)
            .ExecuteUpdateAsync(b => b.SetProperty(x => x.IsConnected, isConnected));

        await _applicationDb.SaveChangesAsync(CancellationToken.None);
    }

    public async Task<List<UserToMatch>> GetNotConnectedUsersFromMatch(Match match)
    {
        return await _applicationDb.UserToMatches.Include(x => x.User).Include(x => x.Match)
            .Where(x => x.Match == match && x.IsActive && x.IsConnected == false).ToListAsync();
    }

    public async Task RemoveUserFromMatch(string userId)
    {
        var userToMatch = await _applicationDb.UserToMatches.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);

        if (userToMatch != null) 
        {
            userToMatch.IsActive = false;

            _applicationDb.UserToMatches.Update(userToMatch);
            await _applicationDb.SaveChangesAsync(CancellationToken.None);
        }
    }

    public async Task<bool> IsUserInMatch(User user)
    {
        return await _applicationDb.UserToMatches.AnyAsync(x => x.User == user && x.IsActive);
    }

    public async Task<UserToMatch?> GetUserPlayDataByTicket(string ticket, string matchId)
    {
        return await _applicationDb.UserToMatches
            .Include(x => x.User)
            .Include(s => s.Match)
            .FirstOrDefaultAsync(x => x.Ticket == ticket && x.IsActive && !x.IsConnected && x.Match.Id == matchId);
    }

    public async Task<Match?> GetMatchByParticipant(string userId)
    {
        IQueryable<UserToMatch> userIQuer = _applicationDb.UserToMatches;

        return await userIQuer
            .Include(x => x.Match)
            .ThenInclude(x => x.Users.Where(u => u.IsActive)).ThenInclude(x => x.User).AsSplitQuery()
            .Where(um => um.User.Id == userId && um.IsActive)
            .Select(x => x.Match)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserToMatch>> GetActiveUsersInMatch(string matchId)
    {
        return await _applicationDb.UserToMatches
            .Include(x => x.User)
            .Include(s => s.Match)
            .Where(x => x.IsActive && x.Match.Id == matchId)
            .ToListAsync();
    }
}
