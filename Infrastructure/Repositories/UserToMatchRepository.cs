using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class UserToMatchRepository : IUserToMatchRepository
{
    private readonly IApplicationDbContext _applicationDb;

    public UserToMatchRepository(IApplicationDbContext applicationDb)
    {
        _applicationDb = applicationDb;
    }

    public async Task<UserToMatch?> AddUserToMatchAsync(User user, Match match, string userIp, PlayerTypeEnum playerType, GameTypeEnum gameType)
    {
        var newUserToMatch = new UserToMatch()
        {
            MatchId = match.Id,
            UserId = user.Id,
            UserIp = userIp,
            UserType = playerType,
            IsActive = true
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

    public async Task<List<UserToMatch>> GetNotConnectedUsersFromMatch(Match match)
    {
        return await _applicationDb.UserToMatches.Include(x => x.User).Include(x => x.Match)
            .Where(x => x.Match == match && x.IsActive && x.IsConnected == false).ToListAsync();
    }

    public async Task RemoveUserFromMatch(User user)
    {
        var userToMatch = await _applicationDb.UserToMatches.FirstOrDefaultAsync(x => x.User == user && x.IsActive);

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
}
