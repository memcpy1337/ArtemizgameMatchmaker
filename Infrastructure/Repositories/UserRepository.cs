using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{

    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _queue;
    private readonly IApplicationDbContext _applicationDb;

    private const string QUEUE_NAME = "queue";

    public UserRepository(IConnectionMultiplexer redis, IApplicationDbContext applicationDb)
    {
        _redis = redis;
        _queue = _redis.GetDatabase();
        _applicationDb = applicationDb;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _applicationDb.Users.AddAsync(user, cancellationToken);

        await _applicationDb.SaveChangesAsync(cancellationToken);

        await _queue.ListRightPushAsync(QUEUE_NAME, user.UserId);
    }

    /// <summary>
    /// return userid from queue
    /// </summary>
    /// <returns></returns>
    public async Task<User?> PopFromQueue()
    {
        var user = await _queue.ListLeftPopAsync(QUEUE_NAME);

        if (user.IsNullOrEmpty)
            return null;

        return await _applicationDb.Users.FirstOrDefaultAsync(x => x.UserId == user.ToString() && x.IsActive, CancellationToken.None);
    }

    public async Task ReturnToQueue(string userId)
    {
        await _queue.ListLeftPushAsync(QUEUE_NAME, userId);
    }

    public bool Any(string userId)
    {
        return _applicationDb.Users.Any(x => x.UserId == userId && x.IsActive);
    }

    public async Task<bool> AnyAsync(string userId)
    {
        return await _queue.KeyExistsAsync(userId);
    }

    public async Task<User?> GetAsync(string userId, CancellationToken cancellationToken)
    {
        return await _applicationDb.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive, cancellationToken);
    }

    public async Task RemoveAsync(string userId)
    {
        await _queue.ListRemoveAsync(QUEUE_NAME, userId);

        var user = await _applicationDb.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);

        if (user == null)
            return;

        user.IsActive = false;

        _applicationDb.Users.Update(user);

        await _applicationDb.SaveChangesAsync(CancellationToken.None);
    }


}
