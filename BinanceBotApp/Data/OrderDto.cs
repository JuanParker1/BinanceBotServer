using System;

namespace BinanceBotApp.Data
{
    public class OrderDto
    {
        /// <summary>
        /// Order id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Requested user id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Order id at exchange
        /// </summary>
        public long OrderId { get; set; }
        
        /// <summary>
        /// Client order id at exchange
        /// </summary>
        public string ClientOrderId { get; set; }
        
        /// <summary>
        /// Trade pair name
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
        /// Order status ("New", for example)
        /// </summary>
        public string Status { get; set; }
        
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
        /// Order creation date
        /// </summary>
        public DateTime DateCreated { get; set; }
        
        /// <summary>
        /// Order closing date
        /// </summary>
        public DateTime? DateClosed { get; set; }
        
        /// <summary>
        /// Auto or manually created order
        /// </summary>
        public string CreationType { get; set; }
        
        /// <summary>
        /// Coin price for the moment when order was created
        /// </summary>
        public double Price { get; set; }
    }
}