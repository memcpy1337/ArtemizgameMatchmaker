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
    private readonly IApplicationDbContext _applicationDb;

    private const string QUEUE_NAME = "queue";

    public UserRepository(IApplicationDbContext applicationDb)
    {
        _applicationDb = applicationDb;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _applicationDb.Users.AddAsync(user, cancellationToken);

        await _applicationDb.SaveChangesAsync(cancellationToken);
    }

    public bool Any(string userId)
    {
        return _applicationDb.Users.Any(x => x.Id == userId);
    }

    public async Task<User?> GetAsync(string userId, CancellationToken cancellationToken)
    {
        return await _applicationDb.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task UpdateUserElo(string userId, int newElo)
    {
        await _applicationDb.Users
        .Where(x => x.Id == userId)
        .ExecuteUpdateAsync(b => b.SetProperty(x => x.Elo, newElo));

        await _applicationDb.SaveChangesAsync(CancellationToken.None);
    }
}
