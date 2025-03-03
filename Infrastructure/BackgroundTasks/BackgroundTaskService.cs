using Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.BackgroundTasks
{
    public class BackgroundTaskService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public BackgroundTaskService(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[BackgroundTaskService] Uruchomiono przetwarzanie kolejki...");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_taskQueue.TryDequeue(out var workItem))
                {
                    try
                    {
                        Console.WriteLine("[BackgroundTaskService] Wykonywanie zadania...");
                        await workItem(stoppingToken);
                        Console.WriteLine("[BackgroundTaskService] Zadanie zakończone!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString() + "[BackgroundTaskService] Błąd podczas wykonywania zadania");
                    }
                }

                await Task.Delay(100); // Krótkie opóźnienie, żeby nie zjadać CPU
            }
        }
    }
}
