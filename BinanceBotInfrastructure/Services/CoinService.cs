using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services.BackgroundWorkers;
using BinanceBotInfrastructure.Services.CoinPricesStorage;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;
        private readonly IOrdersService _ordersService;
        private readonly IEventService _eventService;
        private readonly IWebSocketClientService _webSocketService;
        private readonly ICoinPricesStorage _coinPricesStorage;
        private readonly IRefreshOrderBackgroundQueue _refreshOrderQueue;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private const int _notifyThreshold = 20;
        private static int _counter = 0;

        public CoinService(ISettingsService settingsService, IHttpClientService httpService, 
            IOrdersService ordersService, IEventService eventService, 
            IWebSocketClientService webSocketService, ICoinPricesStorage coinPricesStorage, 
            IRefreshOrderBackgroundQueue refreshOrderQueue)
        {
            _settingsService = settingsService;
            _httpService = httpService;
            _ordersService = ordersService;
            _eventService = eventService;
            _webSocketService = webSocketService;
            _coinPricesStorage = coinPricesStorage;
            _refreshOrderQueue = refreshOrderQueue;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        
        public async Task<IEnumerable<string>> GetTradingPairsAsync(int idUser, 
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);

            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var coinPricesInfo = 
                await _httpService.ProcessRequestAsync<IEnumerable<CoinPrice>>(uri, 
                    new Dictionary<string, string>(), keys,HttpMethods.Get, token);
            
            var filtered =  coinPricesInfo.Select(c => CutTradePairEnding(c.Symbol))
                .Distinct();
            
            return filtered;
        }
        
        public Task SubscribeSingleCoinPriceStreamAsync(string pair, int idUser, 
            Action<string> responseHandler, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[\"{pair.ToLower()}@bookTicker\"],\"id\": 1}}";

                var endpoint = TradeWebSocketEndpoints.GetMainWebSocketEndpoint();
            
                var wsClientWrapper = await _webSocketService.SendAsync(endpoint, data, 
                    idUser, WebsocketConnectionTypes.Price, token);

                if (!wsClientWrapper.IsListening)
                {
                    wsClientWrapper.IsListening = true;
                    await _webSocketService.ListenAsync(wsClientWrapper.WebSocket, 
                        async dictionary => await HandleSingleCoinResponseAsync(responseHandler, 
                            dictionary, token), token);
                }
            }, token);
        }

        public Task SubscribeCoinPricesStreamAsync(IEnumerable<string> pairs, int idUser, 
            Action<string> responseHandler, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var pairsString = string.Join(",", pairs.Select(p => $"\"{p.ToLower()}@bookTicker\""));
                var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";

                var endpoint = TradeWebSocketEndpoints.GetMainWebSocketEndpoint();
            
                var wsClientWrapper = await _webSocketService.SendAsync(endpoint, data, 
                    idUser, WebsocketConnectionTypes.Prices, token);
                
                var userCoinPrices = _coinPricesStorage.Get(idUser);

                if (!wsClientWrapper.IsListening)
                {
                    wsClientWrapper.IsListening = true;
                    await _webSocketService.ListenAsync(wsClientWrapper.WebSocket, 
                        async dictionary => await HandleMultiCoinResponseAsync(idUser, 
                            responseHandler, dictionary, userCoinPrices, token), token);
                }
            }, token);
        }

        public async Task UnsubscribeCoinPriceStreamAsync(IEnumerable<string> pairs, 
            int idUser, WebsocketConnectionTypes connectionType, CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Select(p => $"\"{p.ToLower()}@bookTicker\""));
            var data = $"{{\"method\": \"UNSUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";

            await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, connectionType, token);
        }

        public async Task GetSubscriptionsListAsync(int idUser, CancellationToken token)
        {
            var data = $"{{\"method\": \"LIST_SUBSCRIPTIONS\",\"id\": 1}}";   // TODO: Попробовать через @ и '' обычные кавычки
            
            await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.Prices, token);
        }

        private async Task HandleSingleCoinResponseAsync(Action<string> responseHandler,
            IDictionary<string, string> response, CancellationToken token)
        {
            if(!response.ContainsKey("s") || string.IsNullOrEmpty(response["s"]))
                return;
            
            var tradePair = response["s"];
     
            if (!double.TryParse(response["b"], out var currentPrice))
                return;
            
            responseHandler?.Invoke(JsonSerializer.Serialize(new { Symbol = tradePair, Price = currentPrice },
                _jsonSerializerOptions));

            await Task.FromResult(token); // Just because wsClient.ListenAsync() accepts Task returning response handler.
        }
        
        private async Task HandleMultiCoinResponseAsync(int idUser, Action<string> responseHandler, 
            IDictionary<string, string> response, IDictionary<string, double> highestPrices,
            CancellationToken token)
        {
            if (response.ContainsKey("s") && !string.IsNullOrEmpty(response["s"]))
            {
                _counter++;  // Too much price data, hangs browser page. Only every {_notifyThreshold} price is sent to signalR frontend channel
                if (_counter <= _notifyThreshold) 
                    return;
                _counter = 0;
                await HandleNewCoinPriceAsync(idUser, response, highestPrices,
                    responseHandler, token);
            }

            if(response.ContainsKey("result"))
                responseHandler?.Invoke(JsonSerializer.Serialize(new { Result = response["result"] },
                    _jsonSerializerOptions));
        }
        
        private async Task HandleNewCoinPriceAsync(int idUser, IDictionary<string, string> response,
            IDictionary<string, double> highestPrices, Action<string> responseHandler,
            CancellationToken token)
        {
            if(!response.ContainsKey("s") || string.IsNullOrEmpty(response["s"]))
                return;
            
            var tradePair = response["s"];
     
            if (!double.TryParse(response["b"], out var currentPrice))
                return;
            
            var currentHighestPrice = highestPrices.ContainsKey(tradePair) 
                ? highestPrices[tradePair] 
                : 0D;

            if (currentPrice > currentHighestPrice)
                highestPrices[tradePair] = currentPrice;

            var userSettings = await _settingsService.GetSettingsAsync(idUser, token);

            if (userSettings.IsTradeEnabled)
            {
                _refreshOrderQueue.EnqueueTask(async (token) =>
                {
                    var orderRate = userSettings.LimitOrderRate;
                    await RecreateOrderAsync(idUser, tradePair, currentPrice, 
                        orderRate, token);
                });
            }

            responseHandler?.Invoke(JsonSerializer.Serialize(new { Symbol = tradePair, Price = currentPrice },
                _jsonSerializerOptions));
        }

        private async Task RecreateOrderAsync(int idUser, string tradePair, double currentPrice, 
            int orderLimitRate, CancellationToken token)
        {
            var deletedOrderTemplate = await _eventService.CreateEventTextAsync(EventTypes.OrderCancelled,
                new List<string> {""}, token);
            await _eventService.CreateEventAsync(idUser, deletedOrderTemplate, token);
            
            var formattedTradePair = tradePair.ToUpper();
            var deletedOrdersInfos = await _ordersService.DeleteAllOrdersForPairAsync(idUser, 
                formattedTradePair, 10000, token);
            var totalCoinsAmount = deletedOrdersInfos.Select(o => 
                    double.TryParse(o.OrigQty, out var res) 
                        ? res 
                        : 0.0)
                .Sum();
            var newOrderDto = new NewOrderDto
            {
                IdUser = idUser,
                Symbol = formattedTradePair,
                Side = "BUY",
                Type = "LIMIT",
                TimeInForce = "GTC",
                Quantity = totalCoinsAmount,
                Price = currentPrice - (currentPrice / 100 * orderLimitRate),
                IdCreationType = 1,
                RecvWindow = 10000
            };
            await _ordersService.CreateOrderAsync(newOrderDto, token);
        }

        private static string CutTradePairEnding(string tradePair) =>
            tradePair.EndsWith("USDT") 
                ? tradePair[..^4] 
                : tradePair[..^3];
    }
}