using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotApp.Services
{
    public interface IWebSocketClientService
    {
        Task ConnectToWebSocketAsync(Uri endpoint, string data, int idUser, 
            WebsocketConnectionTypes streamType, Action<string> responseHandler, 
            CancellationToken token);
    }
}