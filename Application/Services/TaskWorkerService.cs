using Application.Common.Interfaces;
using Application.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class TaskWorkerService : ITaskWorkerService
{
    private readonly IRedisTaskRepository _redisTaskRepository;
    private readonly ITaskService _taskService;

    public TaskWorkerService(IRedisTaskRepository redisTaskRepository, ITaskService taskService)
    {
        _redisTaskRepository = redisTaskRepository;
        _taskService = taskService;
    }

    public async Task ExecuteAsync()
    {
        var task = await _redisTaskRepository.GetNextDueTaskAsync(DateTime.Now);

        if (task == (null, null))
            return;

        await _taskService.ProcessTask((TaskType)task.Item1!, task.Item2!);
    }
}
