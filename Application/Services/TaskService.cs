using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.Common.Models.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchService _matchService;
    private readonly ILogger<TaskService> _logger;
    private readonly IUserToMatchRepository _userToMatchRepository;

    public TaskService(IMatchRepository matchRepository, IMatchService matchService, ILogger<TaskService> logger, IUserToMatchRepository userToMatchRepository)
    {
        _matchRepository = matchRepository;
        _matchService = matchService;
        _logger = logger;
        _userToMatchRepository = userToMatchRepository;
    }

    public async Task ProcessTask(TaskType type, string entityId)
    {
        switch(type)
        {
            case TaskType.ServerReadyTimeout:
                await ServerReadyTimeoutHandler(entityId);
                break;
            case TaskType.PlayersReadyTimeout:
                await PlayersReadyTimeoutHandler(entityId);
                break;
        }
    }

    private async Task ServerReadyTimeoutHandler(string matchId)
    {
        var match = await _matchRepository.Get(matchId);
        if (match == null || match.Status != MatchStatusEnum.WaitForServer)
        {
            _logger.LogWarning($"Error timeout for server wait: {matchId}");
            return;
        }

        await _matchService.CancelMatch(matchId, "Server not UP over time");
    }

    private async Task PlayersReadyTimeoutHandler(string matchId)
    {
        var match = await _matchRepository.Get(matchId);
        if (match == null || match.Status != MatchStatusEnum.WaitForPlayers)
        {
            _logger.LogWarning($"Error timeout for players wait: {matchId}");
            return;
        }

        var usersNotConnected = await _userToMatchRepository.GetNotConnectedUsersFromMatch(match);

        if (usersNotConnected.Count == 0)
            return;

        foreach(var user in usersNotConnected)
        {
            await _matchService.RemovePlayerFromMatch(user.User);
        }
    }
}
