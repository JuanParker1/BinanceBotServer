namespace BinanceBotApp.Data
{
    /// <summary>
    /// User token info
    /// </summary>
    public class AuthTokenDto
    {
        /// <summary>
        /// User id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// User token
        /// </summary>
        public string Token { get; set; }
    }
}