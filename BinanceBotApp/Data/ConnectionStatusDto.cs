namespace BinanceBotApp.Data
{
    /// <summary>
    /// Exchange websockets connection status
    /// </summary>
    public class ConnectionStatusDto
    {
        /// <summary>
        /// Coin prices (in user wallet) stream connection status
        /// </summary>
        public bool IsPricesStreamConnected { get; set; }
        
        /// <summary>
        /// Single coin price stream connection status (stream is used
        /// in Create order form to show selected coin price in real time)
        /// </summary>
        public bool IsPriceStreamConnected { get; set; }
        
        /// <summary>
        /// User data (orders, account balance refresh) stream connection status
        /// </summary>
        public bool IsUserDataStreamConnected { get; set; }
    }
}