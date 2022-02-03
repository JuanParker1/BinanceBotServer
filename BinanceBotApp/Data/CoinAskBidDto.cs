namespace BinanceBotApp.Data
{
    /// <summary>
    /// Best ask/bid price for trading pair
    /// </summary>
    public class CoinBestAskBidDto
    {
        /// <summary>
        /// Trading pair name
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// Best sell price
        /// </summary>
        public string BidPrice { get; set; }
        
        /// <summary>
        /// Sell quantity
        /// </summary>
        public string BidQty { get; set; }
        
        /// <summary>
        /// Best ask price
        /// </summary>
        public string AskPrice { get; set; }
        
        /// <summary>
        /// Ask quantity
        /// </summary>
        public string AskQty { get; set; }
    }
}