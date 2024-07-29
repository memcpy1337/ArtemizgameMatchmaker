using Application.Common.DTOs;
using Application.Common.Interfaces;
using MassTransit;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class QueueRepository : IQueueRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _queue;

    private const string QUEUE_NAME = "queue";
    private const string DATA_PREFIX_NAME = "player_data:";

    public QueueRepository(IPublishEndpoint publishEndpoint, IConnectionMultiplexer redis)
    {
        _redis = redis;
        _queue = _redis.GetDatabase();
    }

    public async Task PushAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken)
    {
        var jsonR = JsonConvert.SerializeObject(userQueueRequest);

        await _queue.ListRightPushAsync(QUEUE_NAME, userQueueRequest.UserId);

        await _queue.StringSetAsync($"{DATA_PREFIX_NAME}{userQueueRequest.UserId}", jsonR);
    }

    public async Task<UserQueueRequest?> PopAsync()
    {
        var userId = await _queue.ListLeftPopAsync(QUEUE_NAME);

        if (userId.IsNullOrEmpty)
            return null;

        string? playerData = await _queue.StringGetAsync($"{DATA_PREFIX_NAME}{userId}");

        if (playerData == null)
        {
            return null;
        }

        await _queue.KeyDeleteAsync($"{DATA_PREFIX_NAME}{userId}");

        return JsonConvert.DeserializeObject<UserQueueRequest>(playerData);
    }

    public async Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken)
    {
        long removedFromList = await _queue.ListRemoveAsync(QUEUE_NAME, userId);

        if (removedFromList > 0)
        {
            await _queue.KeyDeleteAsync($"{DATA_PREFIX_NAME}{userId}");
        }
    }

    public async Task<bool> IsInQueue(string userId)
    {
        return await _queue.ListPositionAsync(QUEUE_NAME, userId) != -1;
    }

    public async Task ReturnToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken)
    {
        await PushAsync(userQueueRequest, cancellationToken);
    }
}
