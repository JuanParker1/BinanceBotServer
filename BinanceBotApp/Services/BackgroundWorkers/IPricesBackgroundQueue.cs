﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services.BackgroundWorkers
{
    public interface IPricesBackgroundQueue
    {
        void EnqueueTask(Func<CancellationToken, Task> action);
        bool TryDequeue(out Func<CancellationToken, Task> action);
    }
}