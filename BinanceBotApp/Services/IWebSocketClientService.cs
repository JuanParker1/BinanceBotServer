using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotApp.Services
{
    public interface IWebSocketClientService
    {
        Task<ClientWebSocket> SendAsync(Uri endpoint, string data, int idUser,
            WebsocketConnectionTypes streamType, CancellationToken token);
        Task ListenAsync(ClientWebSocket webSocket, Action<string> responseHandler,
            CancellationToken token);
    }
}