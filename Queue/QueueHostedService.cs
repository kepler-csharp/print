namespace imprimir.Queue;

using Microsoft.Extensions.Hosting;

public class QueueHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly ILogger<QueueHostedService> _logger;

    public QueueHostedService(IBackgroundTaskQueue queue, ILogger<QueueHostedService> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _queue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken); // Worker execution
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker Execution Failed");
            }
        }
    }
}