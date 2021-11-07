using System;
using System.Collections.Generic;
using System.Linq;

namespace BinanceBotApp.DataInternal.Endpoints
{
    // How to manage a local order book correctly:
    // 1. Open a stream to wss://stream.binance.com:9443/ws/bnbbtc@depth.
    // 2. Buffer the events you receive from the stream.
    // 3. Get a depth snapshot from https://api.binance.com/api/v3/depth?symbol=BNBBTC&limit=1000 .
    // 4. Drop any event where u is <= lastUpdateId in the snapshot.
    // 5. The first processed event should have U <= lastUpdateId+1 AND u >= lastUpdateId+1.
    // 6. While listening to the stream, each new event's U should be equal to the previous event's u+1.
    // 7. The data in each event is the absolute quantity for a price level.
    // 8. If the quantity is 0, remove the price level.
    // 9. Receiving an event that removes a price level that is not in your local order book can happen and is normal.
    
    public static class TradeWebSocketEndpoints
    {
        /// <summary>
        /// Gets base websocket uri
        /// </summary>
        /// <returns> Base websocket uri </returns>
        public static Uri GetMainWebSocketEndpoint() =>
            new ($"{GeneralInfo.BaseWebsocketUri}");
        
        /// <summary>
        /// Get any update to the best bid or asks price or quantity in
        /// real-time for a specified symbol.
        /// Update Speed: Real-time
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <returns>Websocket uri</returns>
        public static Uri GetSymbolBestAskBidPrice(string symbol) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                 $"{symbol.ToLower()}@bookTicker");
        
        /// <summary>
        /// Get any update to the best bid or asks price or quantity
        /// in real-time for all symbols.
        /// Update Speed: Real-time
        /// </summary>
        /// <returns>Websocket uri</returns>
        public static Uri GetAllSymbolsBestAskBidPrice() =>
            new ($"{GeneralInfo.BaseWebsocketUri}/!ticker@arr");
        
        /// <summary>
        /// Get the trades stream web socket uri
        /// The Trade Streams push raw trade information.
        /// Each trade has a unique buyer and seller.
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <returns>Websocket uri</returns>
        public static Uri GetTradesWebSocketUri(string symbol) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                 $"{symbol.ToLower()}@trade");
        
        /// <summary>
        /// Get the aggregate stream web socket uri
        /// The Aggregate Trade Streams push trade information
        /// that is aggregated for a single taker order.
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <returns>Websocket uri</returns>
        public static Uri GetAggTradesWebSocketUri(string symbol) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                 $"{symbol.ToLower()}@aggTrade");
        
        /// <summary>
        /// 24hr rolling window mini-ticker statistics.
        /// These are NOT the statistics of the UTC day,
        /// but a 24hr rolling window for the previous 24hrs.
        /// Update Speed: 1000ms
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <returns>Websocket uri</returns>
        public static Uri GetIndividualSymbolMiniTickerUri(string symbol) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                 $"{symbol.ToLower()}@miniTicker");

        /// <summary>
        /// 24hr rolling window ticker statistics for a single symbol.
        /// These are NOT the statistics of the UTC day, but a 24hr
        /// rolling window for the previous 24hrs.
        /// Update Speed: 1000ms
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <returns>Websocket uri</returns>
        public static Uri GetIndividualSymbolTickerUri(string symbol) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                 $"{symbol.ToLower()}@ticker");

        /// <summary>
        /// 24hr rolling window mini-ticker statistics for all symbols
        /// that changed in an array. These are NOT the statistics of the
        /// UTC day, but a 24hr rolling window for the previous 24hrs.
        /// Note that only tickers that have changed will be present in the array.
        /// Update Speed: 1000ms
        /// </summary>
        /// <returns>Websocket uri</returns>
        public static Uri GetAllMArketMiniTickersUri() =>
            new ($"{GeneralInfo.BaseWebsocketUri}/!ticker@arr");
        
        /// <summary>
        /// Get Kline webSocket uri
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <param name="interval"></param>
        /// <returns>Websocket uri</returns>
        public static Uri GetKlineWebSocketEndpoint(string symbol, string interval) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                  $"{symbol.ToLower()}@kline_{interval}");

        /// <summary>
        /// Get top bids and asks. Valid are 5, 10, or 20.
        /// </summary>
        /// <param name="symbol">Coins pair</param>
        /// <param name="levels">Valid are 5, 10, or 20.</param>
        /// <returns>Websocket uri</returns>
        public static Uri GetPartialDepthWebSocketEndpoint(string symbol, int levels) =>
            new ($"{GeneralInfo.BaseWebsocketUri}/" +
                $"{symbol.ToLower()}@depth{levels}");

        /// <summary>
        /// Get combined webSocket uri
        /// </summary>
        /// <param name="symbols">Coins pairs</param>
        /// <returns>Websocket uri</returns>
        public static Uri ConnectToDepthWebSocketCombinedPartial(IEnumerable<string> symbols) =>
            new ($"{GeneralInfo.CombinedWebsocketUri}={MakeCombinedUri(symbols)}");

        private static string MakeCombinedUri(IEnumerable<string> symbols)
        {
            var preparedSymbolsList = symbols.Select(symbol => 
                $"{symbol.ToLower()}@depth/").ToList();

            return string.Join("", preparedSymbolsList);
        }
    }
}