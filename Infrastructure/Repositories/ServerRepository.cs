using Domain.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public sealed class ServerRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    public ServerRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task Add(Server server)
    {
        await _db.HashSetAsync($"Server:{server.Id}",
            new HashEntry[] {
                new HashEntry("Host", server.Ip),
                new HashEntry("Port", server.Port.ToString()),
                new HashEntry("IsReady", server.IsReady.ToString()),
                new HashEntry("MatchId", server.MatchId),
            });
    }


}
