using System;
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

        public RequestTrackerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices.GetRequiredService<IRequestTrackerService>();
            var requestLog = new BinanceBotApp.Data.RequestDto
            {
                Login = context.User?.Identity.Name,
                Ip = context.Connection.RemoteIpAddress.ToString(), // TODO: Внедрить побольше CRUDService. В том числе и сюда. И в создание OrdersHistory (GetAll/Insert) и т.д. Тупо создание и Get entites - все через CRUD
                Date = DateTime.Now,
                RequestMethod = context.Request.Method,
                RequestPath = context.Request.Path.Value,
                Referer = context.Request.Headers["Referer"].ToString(),
            };
            
            var userIdString = context.User?.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                requestLog.IdUser = userId;
            
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                await _next?.Invoke(context);
                stopwatch.Stop();
                requestLog.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                requestLog.Status = context.Response.StatusCode;
                await service.RegisterRequestAsync(requestLog);
            }
            catch (System.Exception ex)
            {
                stopwatch.Stop();
                requestLog.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                requestLog.Status = context.Response.StatusCode;
                await service.RegisterRequestErrorAsync(requestLog, ex);
            }
        }
    }
}