using Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class QueueWorkerService : IQueueWorkerService
{
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IPublishService _publishService;

    public QueueWorkerService(IUserRepository userRepository, IMatchRepository matchRepository, IPublishService publishService)
    {
        _userRepository = userRepository;
        _matchRepository = matchRepository;
        _publishService = publishService;
    }

    public async Task ExecuteAsync()
    {
        var user = await _userRepository.PopFromQueue();

        if (user == null)
            return;

        var match = await _matchRepository.GetMatchForPlayer(user, user.Elo - 1000, user.Elo + 1000);

        if (match == null)
        {
            match = await _matchRepository.CreateAndAdd(user);

            if (match == null)
            {
                await _userRepository.ReturnToQueue(user.UserId);
                return;
            }

            await _publishService.NewMatchCreated(match.MatchId, match.Regime);
        }

        await _publishService.UserAddToMatch(match.MatchId, user.UserId, match.Server != null);

    }
}
