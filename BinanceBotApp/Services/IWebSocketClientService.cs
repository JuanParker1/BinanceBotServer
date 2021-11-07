using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface IWebSocketClientService
    {
        Task ConnectToWebSocketAsync(Uri endpoint, string data,
            Action<string> responseHandler, CancellationToken token);
    }
}