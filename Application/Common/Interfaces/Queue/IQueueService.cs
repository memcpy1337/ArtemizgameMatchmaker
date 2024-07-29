using Application.Common.DTOs;
using Application.Common.Models;
using Netjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IQueueService
{
    Task AddUserToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken);
    Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken);

    Task ReturnToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken);
    Task<UserQueueRequest?> GetUserFromQueueAsync();
    Task<bool> IsInQueue(string userId);
}
