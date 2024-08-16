using Application.Common.DTOs;
using Application.Common.Interfaces;
using Contracts.Common.Helpers;
using Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class QueueWorkerService : IQueueWorkerService
{
    private readonly IQueueService _queueService;
    private readonly IMatchService _matchService;
    private readonly IUserRepository _userRepository;

    public QueueWorkerService(IQueueService queueService, IMatchService matchService, IUserRepository userRepository)
    {
        _queueService = queueService;
        _matchService = matchService;
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync()
    {
        var queueRequests = await _queueService.GetUsersFromQueueAsync(100);

        if (queueRequests == null || queueRequests.Count == 0)
            return;

        Dictionary<Match, List<UserQueueRequest>> dict = [];

        foreach(var queueRequest in queueRequests)
        {
            bool matchFound = false;

            foreach(var data in dict)
            {
                if (data.Key.Regime == queueRequest.GameRegime && 
                    data.Value.Where(x => x.PlayerType == queueRequest.PlayerType).Count() < Helpers.GetTargetPlayersCountForGameType(data.Key.Regime, queueRequest.PlayerType))
                {
                    var avgElo = data.Value.Average(x => x.Elo);

                    if (avgElo > (queueRequest.Elo - 1000) && avgElo < (queueRequest.Elo + 1000))
                    {
                        data.Value.Add(queueRequest);
                        matchFound = true;
                    }
                }
            }
            
            if (!matchFound)
            {
                dict.TryAdd(new Match()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerUserId = queueRequest.UserId,
                    Regime = queueRequest.GameRegime
                }, [queueRequest]);
            }
        }

        foreach(var match in dict)
        {
            //недостаточно игроков
            if (match.Value.Count < Helpers.GetTargetPlayersCountForGameType(match.Key.Regime))
            {
                await _queueService.ReturnToQueueAsync(match.Value, CancellationToken.None);
            }
            else
            {
                await _matchService.CreateMatch(match.Key, match.Value);

                foreach(var user in match.Value)
                {
                    await _queueService.RemoveUserFromQueueAsync(user.UserId, CancellationToken.None);
                }
            }

        }

        dict.Clear();
    }
}
