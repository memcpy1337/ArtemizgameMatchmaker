using Application.Common.Interfaces;
using Application.Common.Models;
using MassTransit;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class RedisTaskRepository : IRedisTaskRepository
{
    private readonly IDatabase _database;
    private const string QUEUE_NAME = "tasks";

    public RedisTaskRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task ScheduleTaskAsync(TaskType taskType, string entityId, DateTime executeAt)
    {
        await RemoveTaskAsync(taskType, entityId);

        double score = executeAt.ToOADate();
        await _database.SortedSetAddAsync(QUEUE_NAME, $"{(int)taskType}:{entityId}", score);
    }

    public async Task RemoveTaskAsync(TaskType taskType, string entityId)
    {
        await _database.SortedSetRemoveAsync(QUEUE_NAME, $"{(int)taskType}:{entityId}");
    }

    public async Task<(TaskType?, string?)> GetNextDueTaskAsync(DateTime now)
    {
        double maxScore = now.ToOADate();
        var tasks = await _database.SortedSetRangeByScoreAsync(QUEUE_NAME, double.NegativeInfinity, maxScore, take: 1);

        if (tasks.Length > 0)
        {
            await _database.SortedSetRemoveAsync(QUEUE_NAME, tasks[0]);

            var parsed = tasks[0].ToString().Split(":");

            return ((TaskType)Convert.ToInt32(parsed[0]), parsed[1]);

        }
        else
        {
            return (null, null);
        }
    }
}

