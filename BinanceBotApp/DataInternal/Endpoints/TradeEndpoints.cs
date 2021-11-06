using System;

namespace BinanceBotApp.DataInternal.Endpoints
{
    /// <summary>
    /// Trade info endpoints.
    /// </summary>
    public static class TradeEndpoints
    {
        /// <summary>
        /// Get all account orders; active, canceled or
        /// filled. Weight: 10
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetAllOrdersStatusEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/allOrders");

        /// <summary>
        /// Create and validate new order new order without
        /// sending it to order engine. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetTestNewOrderEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/order/test");
        
        /// <summary>
        /// Base order endpoint for CRUD operations. Depending on
        /// HTTP method used to create new order, check an
        /// order's status or cancel an active order. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetNewOrderEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/order");

        /// <summary>
        /// Base all orders endpoint.
        /// GET request: gets all open orders on a symbol. Weight: 3
        /// for a single symbol; 40 when the symbol parameter is omitted.
        /// DELETE request: cancels all active orders on a symbol. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetOpenOrdersStatusEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/openOrders");

        /// <summary>
        /// Get trades for a specific account and symbol. Weight: 10
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetAccountTradeListEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/myTrades");
    }
}