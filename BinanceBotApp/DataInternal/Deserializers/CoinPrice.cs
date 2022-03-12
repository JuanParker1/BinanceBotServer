namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// Short coin price info
    /// </summary>
    public class CoinPrice
    {
        /// <summary>
        /// Trade pair
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// Current trade pair price
        /// </summary>
        public string Price { get; set; }
    }
}