using Domain.Entities;
using Netjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task RemoveAsync(string userId);
    Task<User?> GetAsync(string userId, CancellationToken cancellationToken);
    Task<bool> AnyAsync(string userId);
    bool Any(string userId);
    Task<User?> PopFromQueue();
    Task ReturnToQueue(string userId);
}
