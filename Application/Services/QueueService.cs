using Application.Common.DTOs;
using Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class QueueService : IQueueService
{
    private readonly IQueueRepository _queueRepository;
    private readonly IQueuePublisher _queuePublisher;
    private readonly IMatchService _matchService;

    public QueueService(IQueueRepository queueRepository, IQueuePublisher queuePublisher, IMatchService matchService)
    {
        _queuePublisher = queuePublisher;
        _queueRepository = queueRepository;
        _matchService = matchService;
    }

    public async Task AddUserToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken)
    {
        await _queueRepository.PushAsync(userQueueRequest, cancellationToken);
        await _queuePublisher.AddedUserToQueueAsync(userQueueRequest);
    }

    public async Task<List<UserQueueRequest>?> GetUsersFromQueueAsync(int count)
    {
        return await _queueRepository.PopAsync(count);
    }

    public async Task<bool> IsInQueue(string userId)
    {
        return await _queueRepository.IsInQueue(userId);
    }

    public async Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken)
    {
        if (await IsInQueue(userId)) 
        {
            await _queueRepository.RemoveUserFromQueueAsync(userId, cancellationToken);
            await _queuePublisher.RemovedUserFromQueueAsync(userId);
        }
        else
        {
            await _matchService.RemovePlayerFromMatch(userId);
        }
    }

    public async Task ReturnToQueueAsync(List<UserQueueRequest> userQueueRequests, CancellationToken cancellationToken)
    {
        await _queueRepository.ReturnToQueueAsync(userQueueRequests, CancellationToken.None);
    }
}
