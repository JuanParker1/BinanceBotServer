namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// Short coin price info
    /// </summary>
    public class CoinPrice // TODO: Change deserializers (classes) to deserialization into IDictionary
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