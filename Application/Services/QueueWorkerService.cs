using Application.Common.Interfaces;
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
        var queueRequest = await _queueService.GetUserFromQueueAsync();

        if (queueRequest == null)
            return;

        User? user = await _userRepository.GetAsync(queueRequest.UserId, CancellationToken.None);

        if (user == null)
            return;

        await _matchService.AddOrCreate(user, queueRequest.UserIp, queueRequest.GameRegime, queueRequest.PlayerType);
    }
}
