using System;

namespace BinanceBotApp.Data.Analytics
{
    /// <summary>
    /// Trade profit to BTC price history data
    /// </summary>
    public class ProfitToBtcHistoryDto
    {
        /// <summary>
        /// Requested date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Btc price for requested date
        /// </summary>
        public double BtcPrice { get; set; }
        
        /// <summary>
        /// Profit for requested date end
        /// </summary>
        public double Profit { get; set; }
    }
}