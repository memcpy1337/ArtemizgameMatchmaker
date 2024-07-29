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

    public QueueService(IQueueRepository queueRepository, IQueuePublisher queuePublisher)
    {
        _queuePublisher = queuePublisher;
        _queueRepository = queueRepository;
    }

    public async Task AddUserToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken)
    {
        await _queueRepository.PushAsync(userQueueRequest, cancellationToken);
        await _queuePublisher.AddedUserToQueueAsync(userQueueRequest);
    }

    public async Task<UserQueueRequest?> GetUserFromQueueAsync()
    {
        return await _queueRepository.PopAsync();
    }

    public async Task<bool> IsInQueue(string userId)
    {
        return await _queueRepository.IsInQueue(userId);
    }

    public Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task ReturnToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken)
    {
        await _queueRepository.ReturnToQueueAsync(userQueueRequest, CancellationToken.None);
    }
}
