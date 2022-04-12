using System.Collections.Generic;

namespace BinanceBotApp.Data.Analytics
{
    /// <summary>
    /// Trade profit to BTC price data
    /// </summary>
    public class ProfitToBtcDto
    {
        /// <summary>
        /// Current BTC price for request date
        /// </summary>
        public double CurrentBtcPrice { get; set; }
        
        /// <summary>
        /// Btc price direction (goes up or down)
        /// </summary>
        public bool IsBtcPriceTrendUp { get; set; }
        
        /// <summary>
        /// Opened orders for request period amount
        /// </summary>
        public int TotalOrdersOpened { get; set; }
        
        /// <summary>
        /// Closed orders for request period amount
        /// </summary>
        public int TotalOrdersClosed { get; set; }
        
        /// <summary>
        /// Cancelled orders for request period amount
        /// </summary>
        public int TotalOrdersCancelled { get; set; }
        
        /// <summary>
        /// Total calculated profit for request period
        /// </summary>
        public double TotalProfit { get; set; }
        
        /// <summary>
        /// Average order lifetime for requested period
        /// </summary>
        public double AverageOrderLifeTimeMinutes { get; set; }
        
        /// <summary>
        /// Trade profit to BTC price history
        /// </summary>
        public IEnumerable<ProfitToBtcHistoryDto> Data { get; set; }
    }
}