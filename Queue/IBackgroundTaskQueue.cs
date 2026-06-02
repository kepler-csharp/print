namespace imprimir.Queue;

public interface IBackgroundTaskQueue
{
    Task QueueAsync(Func<CancellationToken, ValueTask> workItem);
    
    Task<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}