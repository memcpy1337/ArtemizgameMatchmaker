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

    public MatchRepository(IApplicationDbContext applicationDb)
    {
        _applicationDb = applicationDb;
    }

   public async Task<Match> CreateAndAdd(User player)
   {

        var match = new Match() {
            MatchId = Guid.NewGuid().ToString(),
            DateCreated = DateTime.Now.ToUniversalTime(),
            IsActive = true,
            OwnerUserId = player.UserId,
            Regime = player.Regime
        };

        await _applicationDb.Matches.AddAsync(match);

        player.Match = match;

        _applicationDb.Users.Update(player);

        await _applicationDb.SaveChangesAsync(CancellationToken.None);

        return match;
    }

    public async Task<Match?> GetMatchForPlayer(User player, int eloMin, int eloMax)
    {
        int targetPlayers = Helpers.GetTargetPlayersCountForGameType(player.Regime, player.PlayerType);

        using (var transaction = await _applicationDb.DatabaseFescade.BeginTransactionAsync())
        {
            try
            {
                var request = await _applicationDb.Matches
                    .Include(s => s.Users)
                    .Include(ser => ser.Server)
                    .Where(x => x.IsActive == true &&
                            x.Regime == player.Regime &&
                            x.Users.Count(u => u.PlayerType == player.PlayerType) < targetPlayers)
                    .Select(x => new
                                {
                                    Match = x,
                                    AvgElo = x.Users.Average(u => (double?)u.Elo)
                                })
                    .FirstOrDefaultAsync(x => x.AvgElo >= eloMin && x.AvgElo <= eloMax);

                if (request == null)
                {
                    return null;
                }

                player.Match = request.Match;
                _applicationDb.Users.Update(player);

                await _applicationDb.SaveChangesAsync(CancellationToken.None);
                await transaction.CommitAsync();

                return request.Match;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
    }

}
