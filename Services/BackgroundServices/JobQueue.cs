using System.Threading.Channels;

public class JobQueue : IJobQueue
{
    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> queue;

    public JobQueue()
    {
        // 용량 제한이 없는 큐를 생성합니다.
        queue = Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();
    }

    public async ValueTask EnqueueAsync(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }
        await queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken
    )
    {
        var workItem = await queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}
