using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IRedisTaskRepository
{
    Task ScheduleTaskAsync(TaskType taskType, string entityId, DateTime executeAt);
    Task RemoveTaskAsync(TaskType taskType, string entityId);
    Task<(TaskType?, string?)> GetNextDueTaskAsync(DateTime now);
}
