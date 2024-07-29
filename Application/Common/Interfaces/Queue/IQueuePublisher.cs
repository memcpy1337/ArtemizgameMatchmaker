using Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IQueuePublisher
{
    Task AddedUserToQueueAsync(UserQueueRequest userQueueRequest);
    Task RemovedUserFromQueueAsync(string userId);
}
