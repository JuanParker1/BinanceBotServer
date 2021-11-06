using System;

namespace BinanceBotApp.DataInternal.Endpoints
{
    /// <summary>
    /// Market info endpoints.
    /// </summary>
    public static class MarketDataEndpoints
    {
        /// <summary>
        /// Get current prices for coin or for all coins.
        /// Weight: 1 for one coin given as parameter (string).
        /// Weight: 2 for all coins.
        /// </summary>
        /// <returns>Object(or array) with coin price</returns>
        public static Uri GetCoinsPricesEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/ticker/price");

        /// <summary>
        /// Get best ask/bid for coins or for all coins.
        /// Weight: 1 for one coin given as parameter (string).
        /// Weight: 2 for all coins.
        /// </summary>
        /// <returns>Object(or array) with best current coin ask/bid price</returns>
        public static Uri GetBestAskBidPricesEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/ticker/bookTicker");

        /// <summary>
        /// Get coin ask/bid history. Weight depends on requesting limit:
        /// 5, 10, 20, 50, 100: Weight: 1. 500: Weight: 5.
        /// 1000: Weight: 10. 5000: Weight: 50
        /// </summary>
        /// <returns>Ask/bid coin history</returns>
        public static Uri GetCoinPriceHistoryEndpoint() =>
            new Uri($"{GeneralInfo.ApiBaseUrl}/" +
                    $"{GeneralInfo.ApiVersion3}/depth");
        
        /// <summary>
        /// Get recent price history. Weight: 1.
        /// </summary>
        /// <returns>Ask/bid coin history</returns>
        public static Uri GetRecentPriceHistoryEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/trades");
        
        /// <summary>
        /// Get last 5 mins average price for trading pair. Weight: 1.
        /// </summary>
        /// <returns>Ask/bid coin history</returns>
        public static Uri GetAverageFiveMinPriceEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/avgPrice");
        
        /// <summary>
        /// Current exchange trading rules and symbol information.
        /// Weight: 10.
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetExchangeInfoEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/exchangeInfo");
    }
}