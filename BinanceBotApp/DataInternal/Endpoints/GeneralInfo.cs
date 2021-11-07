namespace BinanceBotApp.DataInternal.Endpoints
{
    /// <summary> 
    /// General library info (base urls, api versions etc)
    /// </summary>
    public static class GeneralInfo
    {
        /// <summary> 
        /// Base URI for Binance API
        /// </summary>
        public const string ApiBaseUrl = "https://api.binance.com/api";
        
        /// <summary> 
        /// Base URI for Binance SAPI
        /// </summary>
        public const string SApiBaseUrl = "https://api.binance.com/sapi";

        /// <summary> 
        /// API version 1
        /// </summary>
        public const string ApiVersion1 = "v1";
        
        /// <summary> 
        /// API version 3
        /// </summary>
        public const string ApiVersion3 = "v3";

        /// <summary> 
        /// Base WebSocket URI for Binance API
        /// </summary>
        public const string BaseWebsocketUri = "wss://stream.binance.com:9443/ws";

        /// <summary>
        /// Combined WebSocket URI for Binance API
        /// </summary>
        public const string CombinedWebsocketUri = "wss://stream.binance.com:9443/stream?streams";
    }
}