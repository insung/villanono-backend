public class QueuedHostedService : BackgroundService
{
    private readonly ILogger<QueuedHostedService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IJobQueue jobQueue;

    public QueuedHostedService(
        ILogger<QueuedHostedService> logger,
        IServiceProvider serviceProvider,
        IJobQueue jobQueue
    )
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.jobQueue = jobQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. 큐에서 '작업 레시피'인 workItem (Func<...>)을 꺼내옵니다.
                var workItem = await jobQueue.DequeueAsync(stoppingToken);

                logger.LogInformation("Dequeued a new work item. Executing...");

                using (var scope = serviceProvider.CreateScope())
                {
                    // 2. 이 workItem을 직접 실행합니다.
                    //    이 workItem 안에서 필요한 repository와 service를 꺼내서 사용할 것입니다.
                    await workItem(scope.ServiceProvider, stoppingToken);
                }

                logger.LogInformation("Work item executed successfully.");
            }
            catch (OperationCanceledException)
            {
                break; // 정상적인 종료
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while executing a background work item.");
            }
        }

        logger.LogInformation("Queued Hosted Service is stopping.");
    }
}
