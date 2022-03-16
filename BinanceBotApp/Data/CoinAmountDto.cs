namespace BinanceBotApp.Data
{
    /// <summary>
    /// Account coin info
    /// </summary>
    public class CoinAmountDto
    {
        /// <summary>
        /// Coin name
        /// </summary>
        public string Asset { get; set; }
        
        /// <summary>
        /// Free amount
        /// </summary>
        public double Free { get; set; }
        
        /// <summary>
        /// Locked amount
        /// </summary>
        public double Locked { get; set; }
    }
}