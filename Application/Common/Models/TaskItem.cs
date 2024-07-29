using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models;

public class TaskItem
{
    public TaskType Type { get; set; }
    public required string EntityId { get; set; }
}

public enum TaskType
{
    ServerReadyTimeout,
    PlayersReadyTimeout
}
