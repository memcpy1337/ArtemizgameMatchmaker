using Application.Common.Interfaces;

namespace QueueWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueWorkerService _queueWorkerService;

        public Worker(ILogger<Worker> logger, IQueueWorkerService queueWorkerService)
        {
            _logger = logger;
            _queueWorkerService = queueWorkerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                await _queueWorkerService.ExecuteAsync();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
