using Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IQueueRepository
{
    Task PushAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken);
    Task<UserQueueRequest?> PopAsync();
    Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken);
    Task ReturnToQueueAsync(UserQueueRequest userQueueRequest, CancellationToken cancellationToken);
    Task<bool> IsInQueue(string userId);
}
