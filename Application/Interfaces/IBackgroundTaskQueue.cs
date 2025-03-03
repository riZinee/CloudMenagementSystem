namespace Application.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        bool TryDequeue(out Func<CancellationToken, Task> workItem);
    }
}
