namespace BinanceBotApp.Data
{
    /// <summary>
    /// Best ask/bid price for trading pair
    /// </summary>
    public class CoinBestAskBidDto
    {
        public string Symbol { get; set; }
        public string BidPrice { get; set; }
        public string BidQty { get; set; }
        public string AskPrice { get; set; }
        public string AskQty { get; set; }
    }
}