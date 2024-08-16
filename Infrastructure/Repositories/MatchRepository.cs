using Application.Common.DTOs;
using Application.Common.Interfaces;
using Contracts.Common.Helpers;
using Contracts.Common.Models.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly IApplicationDbContext _applicationDb;
    private readonly IUserToMatchRepository _userToMatch;

    public MatchRepository(IApplicationDbContext applicationDb, IUserToMatchRepository userToMatch)
    {
        _applicationDb = applicationDb;
        _userToMatch = userToMatch;
    }

    public async Task<Match?> Get(string matchId)
    {
        return await _applicationDb.Matches.Include(u => u.Users.Where(x => x.IsActive)).FirstOrDefaultAsync(x => x.Id == matchId && x.IsActive);
    }

    public async Task UpdateStatus(MatchStatusEnum status, string matchId)
    {
        IQueryable<Match> matches = _applicationDb.Matches;

        await matches.Where(x => x.Id == matchId)
           .ExecuteUpdateAsync(b => b.SetProperty(u => u.Status, status));

        if (status == MatchStatusEnum.End || status == MatchStatusEnum.Cancelled)
        {
            await matches.Where(x => x.Id == matchId)
                .ExecuteUpdateAsync(b => b.SetProperty(u => u.DateFinish, DateTime.Now.ToUniversalTime()));
        }

        await _applicationDb.SaveChangesAsync(CancellationToken.None);
    }

    public async Task<Match> CreateAsync(Match match)
    {
        await _applicationDb.Matches.AddAsync(match);

        await _applicationDb.SaveChangesAsync(CancellationToken.None);

        return match;
    }

    //public async Task<Match?> GetMatchWithParams(int eloMin, int eloMax, PlayerTypeEnum playerType, GameTypeEnum gameType)
    //{
    //    int targetPlayers = Helpers.GetTargetPlayersCountForGameType(gameType, playerType);

    //    var request = await _applicationDb.Matches
    //        .Include(s => s.Users.Where(x => x.IsActive)).ThenInclude(x => x.User)
    //        .Where(x => x.IsActive == true &&
    //                x.Regime == gameType &&
    //                x.Users.Count(u => u.UserType == playerType) < targetPlayers)
    //        .Select(x => new
    //        {
    //            Match = x,
    //            AvgElo = x.Users.Average(u => (double?)u.User.Elo)
    //        })
    //        .FirstOrDefaultAsync(x => x.AvgElo >= eloMin && x.AvgElo <= eloMax && x.Match.Status == MatchStatusEnum.WaitForPlayers);

    //    if (request == null)
    //    {
    //        return null;
    //    }

    //    return request.Match;
    //}
}
