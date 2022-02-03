namespace BinanceBotApp.Data
{
    /// <summary>
    /// Binance api keys info
    /// </summary>
    public class ApiKeysDto
    {
        /// <summary>
        /// Keys owner id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Binance api key
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// Binance api secret key
        /// </summary>
        public string SecretKey { get; set; }
    }
}