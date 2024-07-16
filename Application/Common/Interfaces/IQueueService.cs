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
    Task AddedUserToQueueAsync(string userId, string ticketId, CancellationToken cancellationToken);
    Task RemovedUserFromQueueAsync(string userId, CancellationToken cancellationToken);
}
