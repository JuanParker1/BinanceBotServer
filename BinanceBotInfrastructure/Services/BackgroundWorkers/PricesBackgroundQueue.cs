using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services.BackgroundWorkers;

namespace BinanceBotInfrastructure.Services.BackgroundWorkers
{
    public class PricesBackgroundQueue : IPricesBackgroundQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _tasks = new ();

        public void EnqueueTask(Func<CancellationToken, Task> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _tasks.Enqueue(task);
        }

        public bool TryDequeue(out Func<CancellationToken, Task> task)
            => _tasks.TryDequeue(out task);
    }
}