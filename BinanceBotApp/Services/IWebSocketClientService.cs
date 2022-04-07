using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotApp.Services
{
    public interface IWebSocketClientService
    {
        (WebSocketWrapper prices, WebSocketWrapper price, WebSocketWrapper userData) GetConnections(int idUser);
        bool IsAlive(WebSocket webSocket);
        Task<WebSocketWrapper> SendAsync(Uri endpoint, string data, int idUser, 
            WebsocketConnectionTypes streamType, CancellationToken token);
        Task ListenAsync(ClientWebSocket webSocket, 
            Func<IDictionary<string, string>, Task> responseHandlerAsync, 
            CancellationToken token);
    }
}