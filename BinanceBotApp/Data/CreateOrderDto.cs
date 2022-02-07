namespace BinanceBotApp.Data
{
    /// <summary>
    /// New order creation params
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// Requested user id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Trading pair name
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// Buy/Sell
        /// </summary>
        public string Side { get; set; }
        
        /// <summary>
        /// Order type (LIMIT, MARKET, STOP_LOSS etc)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Order lifetime type depending on its fill (full/partial)
        /// </summary>
        public string TimeInForce { get; set; }
        
        /// <summary>
        /// Specifies the amount of base asset user wants to
        /// buy/sell. (E.g. for BTC/USDT: quantity 1 will buy/sell 1 BTC.)
        /// </summary>
        public double Quantity { get; set; }
        
        /// <summary>
        /// For "market" type orders. Specifies the amount of
        /// asset user wants to buy/sell (E.g. for BTC/USDT: BUY side: the order will
        /// buy as many BTC as quoteOrderQty USDT can. SELL side: the order will sell
        /// as much BTC needed to receive quoteOrderQty USDT.)
        /// </summary>
        public double QuoteOrderQty { get; set; }
        
        /// <summary>
        /// Asset price
        /// </summary>
        public double Price { get; set; }
        
        /// <summary>
        /// A unique id among open orders. Automatically generated if not sent.
        /// Orders with the same newClientOrderID can be accepted only when the previous one is filled,
        /// otherwise the order will be rejected.
        /// </summary>
        public string NewClientOrderId { get; set; }
        
        /// <summary>
        /// Used with STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.
        /// </summary>
        public double StopPrice { get; set; }
        
        /// <summary>
        /// Used with LIMIT, STOP_LOSS_LIMIT, and TAKE_PROFIT_LIMIT to create an iceberg order.
        /// </summary>
        public double IcebergQty { get; set; }
        
        /// <summary>
        /// Set the response JSON. ACK, RESULT, or FULL; MARKET and LIMIT order types default to
        /// FULL, all other orders default to ACK.
        /// </summary>
        public string NewOrderRespType { get; set; }
        
        /// <summary>
        /// Order lifetime in ms. Default is 5000, max is 60000
        /// </summary>
        public int RecvWindow { get; set; }
    }
}