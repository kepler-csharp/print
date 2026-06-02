namespace imprimir.Queue;

using System.Threading.Channels;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public BackgroundTaskQueue()
    {
        _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();
    }

    public async Task QueueAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await _queue.Writer.WriteAsync(workItem);
    }

    public async Task<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }

}