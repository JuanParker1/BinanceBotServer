using BinanceBotApp.Services.BackgroundWorkers;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotInfrastructure.Services.BackgroundWorkers
{
    public class PricesBackgroundService : BackgroundService
    {
        private readonly IPricesBackgroundQueue _tasksQueue;

        public PricesBackgroundService(IPricesBackgroundQueue tasksQueue)
        {
            _tasksQueue = tasksQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_tasksQueue.TryDequeue(out var item))
                        Task.Run(() => item.Invoke(token), token);
                    else
                        await Task.Delay(100, token);
                }
                catch(Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public override async Task StopAsync(CancellationToken token) =>
            await base.StopAsync(token);
    }
}