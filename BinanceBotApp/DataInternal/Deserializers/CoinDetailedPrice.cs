namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// Coin detailed price
    /// </summary>
    public class CoinDetailedPrice
    {
        /// <summary>
        /// Order book updateId
        /// </summary>
        public long U { get; set; }
        
        /// <summary>
        /// Trade pair
        /// </summary>
        public string s { get; set; }
        
        /// <summary>
        /// Best bid price
        /// </summary>
        public string b { get; set; }
        
        /// <summary>
        /// Best bid qty
        /// </summary>
        public string B { get; set; }
        
        /// <summary>
        /// Best ask price
        /// </summary>
        public string a { get; set; }
        
        /// <summary>
        /// Best ask qty
        /// </summary>
        public string A { get; set; }
    }
}