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

    public async Task<List<UserQueueRequest>?> PopAsync(int count)
    {
        var userIds = await _queue.ListLeftPopAsync(QUEUE_NAME, count);

        if (userIds == null || userIds.Length == 0)
            return null;

        List<UserQueueRequest> output = [];

        foreach(var userId in userIds)
        {
            string? playerData = await _queue.StringGetAsync($"{DATA_PREFIX_NAME}{userId}");

            if (playerData == null)
            {
                continue;
            }

            //await _queue.KeyDeleteAsync($"{DATA_PREFIX_NAME}{userId}");

            var requestData = JsonConvert.DeserializeObject<UserQueueRequest>(playerData);

            if (requestData != null)
                output.Add(requestData);
        }

        return output;
    }

    public async Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken)
    {
        long removedFromList = await _queue.ListRemoveAsync(QUEUE_NAME, userId);

        await _queue.KeyDeleteAsync($"{DATA_PREFIX_NAME}{userId}");
    }

    public async Task<bool> IsInQueue(string userId)
    {
        return await _queue.KeyExistsAsync($"{DATA_PREFIX_NAME}{userId}");
    }


    public async Task ReturnToQueueAsync(List<UserQueueRequest> userQueueRequests, CancellationToken cancellationToken)
    {
        foreach (var userQueueRequest in userQueueRequests)
        {
            await _queue.ListRightPushAsync(QUEUE_NAME, userQueueRequest.UserId);
        }
    }
}
