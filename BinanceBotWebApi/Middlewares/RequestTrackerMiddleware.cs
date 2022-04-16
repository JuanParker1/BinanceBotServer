using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using BinanceBotApp.Services;

namespace BinanceBotWebApi.Middlewares
{
    public class RequestTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private const int _semaphoreTimeout = 5000;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public RequestTrackerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            await _next?.Invoke(context);
            stopwatch.Stop();
            
            var userIdString = context.User?.FindFirst("id")?.Value;
            
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out var userId)
                                                    && userId > 0)
            {
                var service = context.RequestServices.GetRequiredService<IRequestTrackerService>();
                var requestLog = new BinanceBotApp.Data.RequestDto
                {
                    Login = context.User?.Identity?.Name,
                    Ip = context.Connection?.RemoteIpAddress?.ToString(),
                    Date = DateTime.Now,
                    RequestMethod = context.Request.Method,
                    RequestPath = context.Request.Path.Value,
                    Referer = context.Request.Headers["Referer"].ToString(),
                };

                requestLog.IdUser = userId;
                requestLog.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                requestLog.Status = context.Response.StatusCode;
                await SyncAsync(async (token) => 
                    await service.RegisterRequestAsync(requestLog, token));
            }
        }
        
        /// <summary>
        /// Runs delegate as atomic operation. Delegates are synchronized by
        /// SemaphoreSlim object.
        /// </summary>
        /// <param name="funcAsync"> Semaphore synchronized function </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Result of func(..) </returns>
        private static async Task<T> SyncAsync<T>(Func<CancellationToken, Task<T>> funcAsync,
            CancellationToken token = default)
        {
            T result = default;

            try
            {
                if (funcAsync is null || !await _semaphore.WaitAsync(_semaphoreTimeout, token))
                    throw new Exception("Sync function was null or " +
                                        "semaphore wait timeout was exceeded.");
                result = await funcAsync.Invoke(token);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss:fff} error in Request tracker.");
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
            finally
            {
                _semaphore.Release();
            }

            return result;
        }
    }
}