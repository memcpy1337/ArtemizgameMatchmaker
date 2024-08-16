using Application.Common.DTOs;
using Application.Common.Interfaces;
using Contracts.Common.Helpers;
using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using Domain.Calculations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserToMatchRepository _userToMatchRepository;
    private readonly IMatchPublisher _matchPublisher;
    private readonly ILogger<MatchService> _logger;
    private readonly IRedisTaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    private readonly DatabaseFacade _databaseFacade;

    public MatchService(IMatchRepository matchRepository, IRedisTaskRepository taskRepository, IUserToMatchRepository userToMatchRepository, IApplicationDbContext applicationDb, IMatchPublisher matchPublisher, ILogger<MatchService> logger, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userToMatchRepository = userToMatchRepository;
        _databaseFacade = applicationDb.DatabaseFescade;
        _matchPublisher = matchPublisher;
        _logger = logger;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task CreateMatch(Match match, List<UserQueueRequest> players)
    {
        using (var transaction = await _databaseFacade.BeginTransactionAsync())
        {
            try
            {
                match.DateCreated = DateTime.Now.ToUniversalTime();
                match.IsActive = true;
                match.Status = MatchStatusEnum.WaitForServer;

                foreach(var player in players)
                {
                    match.Users.Add(new UserToMatch()
                    {
                        Ticket = player.Ticket.ToString(),
                        IsActive = true,
                        IsConnected = false,
                        UserId = player.UserId,
                        MatchId = match.Id,
                        UserIp = player.UserIp
                    });
                }

                await _matchRepository.CreateAsync(match);

                await transaction.CommitAsync();

                await _matchPublisher.NewMatchReady(match.Id, match.Regime, match.Users.Select(x => x.UserIp).ToList());

                await SetTimeouts(MatchStatusEnum.WaitForPlayers, MatchStatusEnum.WaitForServer, match.Id);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error AddOrCreate {ex.Message}");
            }
        }
    }

    //private async Task AddUserToMatch(User user, Match match, string ticket, string userIp, GameTypeEnum gameType, PlayerTypeEnum playerType)
    //{
    //    var usr = await _userToMatchRepository.AddUserToMatchAsync(user, match, ticket, userIp, playerType, gameType);

    //    if (usr != null)
    //    {
    //        match.Users.Add(usr);

    //        await _matchPublisher.UserAddToMatch(match.MatchId, user.UserId, match.Status);
    //    }
    //}

    public async Task RemovePlayerFromMatch(string userId)
    {

           var match = await _userToMatchRepository.GetMatchByParticipant(userId);

            if (match != null)
            {
                var user = match.Users.Single(x => x.User.Id == userId);

                switch (match.Status)
                {
                    case MatchStatusEnum.WaitForPlayers:
                        if (match.Users.Count > 1)
                        {
                            await _userToMatchRepository.RemoveUserFromMatch(userId);
                            await _matchPublisher.UserRemoveFromMatch(user.User.Id, match.Id);
                        }
                        else //exit last player
                        {
                            await CancelMatch(match.Id, MatchCancelEnum.SomePlayersNotConnected);
                        }

                        break;
                    case MatchStatusEnum.WaitForServer:
                        await CancelMatch(match.Id, MatchCancelEnum.SomePlayersNotConnected);
                        break;
                    case MatchStatusEnum.WaitPlayerConnect:
                        // игрок мог подключиться из за этого произошло отключение от матчмейкера
                        break;
                }


            
        }
        // publish event
    }

    public async Task<bool> UpdateMatchStatus(MatchStatusEnum matchStatus, string matchId)
    {
        var match = await _matchRepository.Get(matchId);

        if (match == null)
        {
            _logger.LogError("Update match status failed: match not found");
            return false;
        }

        await SetTimeouts(match.Status, matchStatus, matchId);

        await _matchRepository.UpdateStatus(matchStatus, match.Id);

        return true;
    }

    private async Task SetTimeouts(MatchStatusEnum prev, MatchStatusEnum next, string matchId)
    {
        switch (next)
        {
            case MatchStatusEnum.WaitForServer:
                await _taskRepository.ScheduleTaskAsync(Common.Models.TaskType.ServerReadyTimeout, matchId, DateTime.Now.AddMinutes(1));
                break;
            case MatchStatusEnum.WaitPlayerConnect:
                await _taskRepository.RemoveTaskAsync(Common.Models.TaskType.ServerReadyTimeout, matchId);
                await _taskRepository.ScheduleTaskAsync(Common.Models.TaskType.PlayersReadyTimeout, matchId, DateTime.Now.AddSeconds(30));
                break;
            default:
                await _taskRepository.RemoveTaskAsync(Common.Models.TaskType.ServerReadyTimeout, matchId);
                await _taskRepository.RemoveTaskAsync(Common.Models.TaskType.PlayersReadyTimeout, matchId);
                break;
        }
    }

    public async Task CancelMatch(string matchId, MatchCancelEnum matchCancel)
    {
        using (var transaction = await _databaseFacade.BeginTransactionAsync())
        {
            _logger.LogInformation($"Cancel match. Reason: {matchCancel.ToString()}");
            try
            {
                var match = await _matchRepository.Get(matchId);

                if (match == null)
                {
                    _logger.LogError("Cancel match failed: match not found");
                    return;
                }

                await _matchRepository.UpdateStatus(MatchStatusEnum.Cancelled, match.Id);

                await _userToMatchRepository.ClearAllUsersFromMatch(match);

                await transaction.CommitAsync();

                await _matchPublisher.MatchCancelled(matchId, matchCancel);

            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error AddOrCreate {ex.Message}");
            }
        }
    }

    private async Task MatchBegin(string matchId)
    {
        if (await UpdateMatchStatus(MatchStatusEnum.Process, matchId))
        {
            await _matchPublisher.MatchStart(matchId);
        }
        //Else
    }

    public async Task UserConnectedToMatch(string userId)
    {
        await _userToMatchRepository.SetUserConnectionStatus(userId, true);

        var match = await _userToMatchRepository.GetMatchByParticipant(userId);

        if (match == null)
            return;

        if (match.Users.Where(x => x.IsConnected).Count() == Helpers.GetTargetPlayersCountForGameType(match.Regime))
        {
            await MatchBegin(match.Id);
        }
    }

    public async Task UserDisconnectedFromMatch(string userId)
    {
        await _userToMatchRepository.SetUserConnectionStatus(userId, false);

        var match = await _userToMatchRepository.GetMatchByParticipant(userId);

        if (match == null)
            return;

        if (!match.Users.Where(x => x.IsConnected).Any() && (match.Status != MatchStatusEnum.End && match.Status != MatchStatusEnum.Cancelled))
        {
            await CancelMatch(match.Id, MatchCancelEnum.NoPlayerInMatch);
        }
    }

    public async Task MatchEnd(string matchId, PlayerTypeEnum wonSide)
    {
        var match = await _matchRepository.Get(matchId);

        if (match == null)
        {
            _logger.LogError("End match failed: match not found");
            return;
        }

        List<MatchPlayerResult> results = [];

        using (var transaction = await _databaseFacade.BeginTransactionAsync())
        {
            var users = await _userToMatchRepository.GetActiveUsersInMatch(matchId);

            var crewTeamSumm = users.Where(x => x.UserType == PlayerTypeEnum.Crew).Select(x => x.User.Elo).Average();
            var hunterTeamSumm = users.Where(x => x.UserType == PlayerTypeEnum.Hunter).Select(x => x.User.Elo).Average();

            foreach (var userInMatch in users)
            {
                var oppositeMmr = userInMatch.UserType == PlayerTypeEnum.Crew ? hunterTeamSumm : crewTeamSumm;

                double userWinProbability = EloCalculation.CalculateProbability(userInMatch.User.Elo, oppositeMmr);
                int newPlayerRating = (int)Math.Ceiling(EloCalculation.UpdateRating(userInMatch.User.Elo, userInMatch.UserType == wonSide ? 1 : 0, userWinProbability));

                results.Add(new MatchPlayerResult() { 
                    UserId = userInMatch.User.Id,
                    EloRatingBefore = userInMatch.User.Elo, 
                    EloRatingAfter = newPlayerRating
                });

                await _userRepository.UpdateUserElo(userInMatch.User.Id, newPlayerRating);
            }

            await _matchRepository.UpdateStatus(MatchStatusEnum.End, matchId);

            await _userToMatchRepository.ClearAllUsersFromMatch(match);

            await transaction.CommitAsync();
        }

        await _matchPublisher.MatchEnd(matchId, results);
    }
}
