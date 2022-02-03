namespace BinanceBotApp.Data
{
    /// <summary>
    /// User authentication info
    /// </summary>
    public class AuthDto
    {
        /// <summary>
        /// User authentication login
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// User authentication password
        /// </summary>
        public string Password { get; set; }
    }
}