using Application.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.BackgroundTasks
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            Console.WriteLine("[BackgroundTaskQueue] Dodano zadanie do kolejki.");
            _workItems.Enqueue(workItem);
        }

        public bool TryDequeue(out Func<CancellationToken, Task> workItem)
        {
            return _workItems.TryDequeue(out workItem);
        }
    }
}
