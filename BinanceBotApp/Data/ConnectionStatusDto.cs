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
        /// User data (orders, account balance refresh) stream connection status
        /// </summary>
        public bool IsUserDataStreamConnected { get; set; }
    }
}