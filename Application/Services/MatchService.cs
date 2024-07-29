using Application.Common.Interfaces;
using Contracts.Common.Helpers;
using Contracts.Common.Models.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserToMatchRepository _userToMatchRepository;
    private readonly IMatchPublisher _matchPublisher;
    private readonly ILogger<MatchService> _logger;
    private readonly IRedisTaskRepository _taskRepository;

    private readonly DatabaseFacade _databaseFacade;

    public MatchService(IMatchRepository matchRepository, IRedisTaskRepository taskRepository, IUserToMatchRepository userToMatchRepository, IApplicationDbContext applicationDb, IMatchPublisher matchPublisher, ILogger<MatchService> logger)
    {
        _matchRepository = matchRepository;
        _userToMatchRepository = userToMatchRepository;
        _databaseFacade = applicationDb.DatabaseFescade;
        _matchPublisher = matchPublisher;
        _logger = logger;
        _taskRepository = taskRepository;
    }

    public async Task AddOrCreate(User user, string userIp, GameTypeEnum gameType, PlayerTypeEnum playerType)
    {
        using (var transaction = await _databaseFacade.BeginTransactionAsync())
        {
            try
            {
                var targetMatch = await GetMatchForPlayer(user, gameType, playerType);

                targetMatch ??= await CreateMatch(gameType, user);

                await AddUserToMatch(user, targetMatch, userIp, gameType, playerType);

                await transaction.CommitAsync();

                if (targetMatch.Users.Count == Helpers.GetTargetPlayersCountForGameType(gameType))
                {
                    await _matchPublisher.NewMatchReady(targetMatch.MatchId, gameType, targetMatch.Users.Select(x => x.UserIp).ToList());
                }
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error AddOrCreate {ex.Message}");
            }
        }
    }

    private async Task<Match?> GetMatchForPlayer(User user, GameTypeEnum gameType, PlayerTypeEnum playerType)
    {
        var match = await _matchRepository.GetMatchWithParams(user.Elo - 1000, user.Elo + 1000, playerType, gameType);

        return match;
    }

    private async Task AddUserToMatch(User user, Match match, string userIp, GameTypeEnum gameType, PlayerTypeEnum playerType)
    {
        var usr = await _userToMatchRepository.AddUserToMatchAsync(user, match, userIp, playerType, gameType);

        if (usr != null)
        {
            match.Users.Add(usr);

            await _matchPublisher.UserAddToMatch(match.MatchId, user.UserId, match.Status);
        }
    }

    public async Task RemovePlayerFromMatch(User user)
    {
        await _userToMatchRepository.RemoveUserFromMatch(user);
        // publish event
    }

    public async Task UpdateMatchStatus(MatchStatusEnum matchStatus, string matchId)
    {
        var match = await _matchRepository.Get(matchId);

        if (match == null)
        {
            _logger.LogError("Update match status failed: match not found");
            return;
        }

        await SetTimeouts(match.Status, matchStatus, matchId);

        await _matchRepository.UpdateStatus(matchStatus, match.MatchId);

        await _matchPublisher.MatchStatusUpdate(matchId, matchStatus);
    }

    private async Task SetTimeouts(MatchStatusEnum prev, MatchStatusEnum next, string matchId)
    {
        switch (next)
        {
            case MatchStatusEnum.WaitForServer:
                await _taskRepository.ScheduleTaskAsync(Common.Models.TaskType.ServerReadyTimeout, matchId, DateTime.Now.AddMinutes(1));
                break;
            case MatchStatusEnum.WaitForPlayers:
                await _taskRepository.RemoveTaskAsync(Common.Models.TaskType.ServerReadyTimeout, matchId);
                await _taskRepository.ScheduleTaskAsync(Common.Models.TaskType.PlayersReadyTimeout, matchId, DateTime.Now.AddSeconds(30));
                break;
        }
    }

    public async Task CancelMatch(string matchId, string reasonMsg)
    {
        using (var transaction = await _databaseFacade.BeginTransactionAsync())
        {
            _logger.LogInformation($"Cancel match. Reason: {reasonMsg}");
            try
            {
                var match = await _matchRepository.Get(matchId);

                if (match == null)
                {
                    _logger.LogError("Cancel match failed: match not found");
                    return;
                }

                await _matchRepository.UpdateStatus(MatchStatusEnum.Cancelled, match.MatchId);

                await _userToMatchRepository.ClearAllUsersFromMatch(match);

                await transaction.CommitAsync();

                await _matchPublisher.MatchCancelled(matchId, reasonMsg);

            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error AddOrCreate {ex.Message}");
            }
        }


    }

    private async Task<Match> CreateMatch(GameTypeEnum gameType, User user)
    {
        var newMatch = await _matchRepository.CreateAsync(gameType, user);
        return newMatch;
    }


}
