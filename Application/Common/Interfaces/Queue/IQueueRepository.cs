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
    Task<List<UserQueueRequest>?> PopAsync(int count);
    Task RemoveUserFromQueueAsync(string userId, CancellationToken cancellationToken);
    Task ReturnToQueueAsync(List<UserQueueRequest> userQueueRequests, CancellationToken cancellationToken);
    Task<bool> IsInQueue(string userId);
}
